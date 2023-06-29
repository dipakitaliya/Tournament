using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using CGRPG_Tournament.Helpers;
using CGRPG_Tournament.Models;
using CGRPG_TournamentLib.Contexts;
using CGRPG_TournamentLib.Helpers;
using CGRPG_TournamentLib.Models;
using Npgsql;

namespace CGRPG_TournamentLib.Controllers
{
    [RoutePrefix("api/tournament")]
    public class TournamentController : ApiController
    {
        private readonly TournamentContext _database = new TournamentContext();

        [HttpPost]
        [ActionName("join")]
        public async Task<IHttpActionResult> JoinTournament([FromBody] JoinUserModel model)
        {
            try
            {
                var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
                if (validationToken.success)
                {
                    /*var settingsModel = new SettingsModel();
                    var conn = _database.Connection;
                    await conn.OpenAsync();
                    {
                        using var cmd = new NpgsqlCommand($"select * from postgres.public.settings", conn);
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            settingsModel.Parse(reader);
                        }
                    }
                    await conn.CloseAsync();
                    var rightNow = DateTime.UtcNow;
                    if (rightNow.CompareTo(settingsModel.TournamentStartDate) > 0)
                    {
                        return BadRequest("The join endpoint duration expired");
                    }*/

                    var newUser = new UserModel
                    {
                        EloRating = 1000.0f,
                        Username = validationToken.username,
                        BattleTickets = 5,
                        Timestamp = DateTime.Now,
                        MetaMaskAddress = validationToken.mm_address,
                        Units = model.Units,
                        BattlePower = model.BattlePower,
                        Wins = 0,
                        Losses = 0,
                        TotalBattles = 0
                    };
                    if (await _database.CheckIfRecordExists("usermodel", "mm_address",
                            "'" + validationToken.mm_address + "'"))
                    {
                        _database.Users.Update(newUser);
                        await _database.SaveChangesAsync();
                    }
                    else
                    {
                        _database.Users.Add(newUser);
                        await _database.SaveChangesAsync();
                    }

                    return Ok();
                }

                return Unauthorized();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ActionName("updateBattlePower")]
        public async Task<IHttpActionResult> UpdateBattlePower([FromBody] UpdateBattlePowerModel model)
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var userA = await _database.Users.SingleOrDefault("mm_address", "'" + validationToken.mm_address + "'");
                if (userA != null)
                {

                    userA.BattlePower = model.BattlePower;
                    _database.Users.Update(userA);
                    await _database.SaveChangesAsync();
                    return Ok();
                }
                return BadRequest("No user data was found");
            }
            return Unauthorized();
        }

        [HttpPost]
        [ActionName("resetMissingTokens")]
        public async Task<IHttpActionResult> ResetMissingTokens([FromBody] ResetMissingTokensModel model)
        {
            try
            {
                var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
                if (validationToken.success)
                {
                    var userA = await _database.Users.SingleOrDefault("mm_address",
                        "'" + validationToken.mm_address + "'");
                    if (userA != null)
                    {
                        int[] nums = Array.ConvertAll(userA.Units?.Split(','), int.Parse);
                        int tokenId = 4000000;

                        foreach (var i in model.tokens)
                        {
                            nums[i] = tokenId;
                        }

                        userA.Units = String.Join(",", nums);
                        userA.BattlePower = model.battlepower;
                        _database.Users.Update(userA);
                        await _database.SaveChangesAsync();
                    }

                    return Ok();
                }

                return Unauthorized();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ActionName("attackPlayer")]
        public async Task<IHttpActionResult> AttackPlayer([FromBody] AttackPlayerModel model)
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var battle = await _database.Battles.SingleOrDefault("battle_id", "'" + model.BattleId + "'");
                if (battle == null)
                {
                    return BadRequest("Battle is either nonexistent or invalid");
                }

                if (battle.IsBattleConcluded)
                {
                    return BadRequest("Battle was already finished");
                }

                var userA = await _database.Users.SingleOrDefault("mm_address", "'" + model.MetaMaskAddress + "'");
                var userB = await _database.Users.SingleOrDefault("mm_address",
                    "'" + model.MetaMaskAddressOpponent + "'");
                if (userA != null && userB != null)
                {
                    const int k = 30;
                    var elo = EloCalculator.Rating(userA.EloRating, userB.EloRating, k, model.DidIWin);
                    userA.EloRating = elo.Item1;
                    userB.EloRating = elo.Item2;
                    //Player A won
                    userA.TotalBattles += 1;
                    if (model.DidIWin)
                    {
                        userA.Wins += 1;
                    }
                    else
                    {
                        userA.Losses += 1;
                    }

                    battle.IsBattleFlagged = model.DidIWin
                        ? (float)userA.BattlePower * 1.3 < (float)userB.BattlePower
                        : (float)userB.BattlePower * 1.3 < (float)userA.BattlePower;
                    battle.IsBattleConcluded = true;
                    _database.Battles.Update(battle);
                    _database.Users.Update(userA);
                    _database.Users.Update(userB);
                    await _database.SaveChangesAsync();
                    return Ok();
                }

                return BadRequest("One of the users were not found");
            }

            return Unauthorized();
        }

        [HttpPost]
        [ActionName("startBattle")]
        public async Task<IHttpActionResult> StartBattle([FromBody] StartBattleModel model)
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                try
                {
                    var mmA = "'" + model.MetaMaskAddress + "'";
                    var mmB = "'" + model.MetaMaskAddressOpponent + "'";
                    var userA = await _database.Users.SingleOrDefault("mm_address", mmA);
                    var userB = await _database.Users.SingleOrDefault("mm_address", mmB);
                    if (userA != null && userB != null)
                    {
                        if (userA.BattleTickets > 0)
                        {
                            userA.BattleTickets -= 1;
                            var battle = new BattleModel()
                            {
                                BattleId = Guid.NewGuid(),
                                MetaMaskAddress = userA.MetaMaskAddress,
                                MetaMaskAddressOpponent = userB.MetaMaskAddress,
                                BattlePower = userA.BattlePower,
                                BattlePowerOpponent = userB.BattlePower,
                                OnCreated = DateTime.Now,
                                IsBattleConcluded = false,
                                IsBattleFlagged = false,
                            };
                            _database.Battles.Add(battle);
                            _database.Users.Update(userA);
                            await _database.SaveChangesAsync();
                            return Content(HttpStatusCode.OK, battle.BattleId.ToString());
                        }

                        return BadRequest("Not enough battle tickets");
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (e.InnerException is { InnerException: { } })
                    {
                        Console.WriteLine(e.InnerException.InnerException.Message);
                    }

                    return BadRequest("An exception was thrown");
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("getUser")]
        public async Task<IHttpActionResult> GetUserData()
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var userA = await _database.Users.SingleOrDefault("mm_address", "'" + validationToken.mm_address + "'");
                if (userA != null)
                {
                    return Content(HttpStatusCode.OK, userA);
                }

                return BadRequest("No user data was found");
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("listOpponents")]
        //[CompressFilter]
        public async Task<IHttpActionResult> ListOpponents()
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var users = _database.Users.ToListAsync().Result.OrderByDescending(i => i.EloRating).ToList();

                //if only 3 players joined tournament 
                if (users.Count() < 3)
                {
                    return Content(HttpStatusCode.OK, users);
                }

                var currentUserIndex =
                    users.IndexOf(users.SingleOrDefault(i => i.MetaMaskAddress == validationToken.mm_address));

                var results = users.Take(3).ToList();
                results.AddRange(users.Skip(currentUserIndex - 3).Take(3));
                results.AddRange(users.Skip(currentUserIndex).Take(5));

                return Content(HttpStatusCode.OK, results);
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("listUserBattles")]
        public async Task<IHttpActionResult> GetListOfUserBattles()
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var battles = await _database.Battles.ToListAsync();
                var result = battles.Where(e => e.MetaMaskAddress.Contains(validationToken.mm_address));
                var battleModels = result.ToList();
                var returnValue = new { Count = battleModels.Count(), Result = battleModels };
                return Content(HttpStatusCode.OK, returnValue);
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("listAllUsers")]
        //[CompressFilter]  
        public async Task<IHttpActionResult> GetListOfAllUsers()
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var userModel = await _database.Users.ToListAsync();
                return Content(HttpStatusCode.OK, userModel);
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("getUserBattleTickets")]
        public async Task<IHttpActionResult> GetUserBattleTickets()
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                var userA = await _database.Users.Single("mm_address", "'" + validationToken.mm_address + "'");
                if (userA != null)
                {
                    return Content(HttpStatusCode.OK, userA.BattleTickets);
                }

                return BadRequest();
            }

            return Unauthorized();
        }

        [HttpPost]
        [ActionName("updateUserBattleTickets")]
        public async Task<IHttpActionResult> UpdateBattleTickets([FromBody] string username)
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                return await UpdateBattleTicketsInternal(Request.Headers.Authorization.Parameter,
                    validationToken.mm_address, 1);
                /*var conn = _database.Connection;
                var settingsModel = new SettingsModel();
                await conn.OpenAsync();
                {
                    using var cmd = new NpgsqlCommand($"select * from settings", conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        settingsModel.Parse(reader);
                    }
                }
                await conn.CloseAsync();
                long entryFee = settingsModel.BattleTicketPrice;
                var balance = await ChilliConnectHelper.GetBalance(Request.Headers.Authorization.Parameter);
                if (balance.Balances[0].Balance >= entryFee && balance.Balances[0].Key == "FIAT")
                {
                    var user = await _database.Users.SingleOrDefault("mm_address", "'" + validationToken.mm_address + "'");
                    if (user != null)
                    {
                        user.BattleTickets += settingsModel.BattleTicketCount;
                        _database.Users.Update(user);
                        await _database.SaveChangesAsync();
                        await ChilliConnectHelper.SetBalance(Request.Headers.Authorization.Parameter, 0, -entryFee);
                        await conn.OpenAsync();
                        {
                            using var cmd = new NpgsqlCommand($"update stats set " +
                                                              $"totalTicketsPurchased = totalTicketsPurchased + {settingsModel.BattleTicketCount}," +
                                                              $"totalFiatConsumed = totalFiatConsumed + {settingsModel.BattleTicketPrice}", conn);
                            await cmd.ExecuteNonQueryAsync();
                        }
                        await conn.CloseAsync();
                        return Ok();
                    }
                    return BadRequest();
                }

                return BadRequest("Not enough balance");*/
            }

            return Unauthorized();
        }

        [HttpPost]
        [ActionName("updateUserBattleTicketsMultiplier")]
        public async Task<IHttpActionResult> UpdateBattleTicketsMultiplier([FromBody] int multiplier)
        {
            var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
            if (validationToken.success)
            {
                return await UpdateBattleTicketsInternal(Request.Headers.Authorization.Parameter,
                    validationToken.mm_address, multiplier);
            }

            return Unauthorized();
        }

        public async Task<IHttpActionResult> UpdateBattleTicketsInternal(string token, string mm_address,
            int multiplier)
        {

            var conn = _database.Connection;
            var settingsModel = new SettingsModel();
            await conn.OpenAsync();
            {
                using var cmd = new NpgsqlCommand($"select * from settings", conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    settingsModel.Parse(reader);
                }
            }
            await conn.CloseAsync();
            long entryFee = settingsModel.BattleTicketPrice * multiplier;
            var balance = await ChilliConnectHelper.GetBalance(token);
            if (balance.Balances[0].Balance >= entryFee && balance.Balances[0].Key == "FIAT")
            {
                var user = await _database.Users.SingleOrDefault("mm_address", "'" + mm_address + "'");
                if (user != null)
                {
                    user.BattleTickets += settingsModel.BattleTicketCount * multiplier;
                    _database.Users.Update(user);
                    await _database.SaveChangesAsync();
                    await ChilliConnectHelper.SetBalance(token, 0, -entryFee);
                    await conn.OpenAsync();
                    {
                        using var cmd = new NpgsqlCommand($"update stats set " +
                                                          $"totalTicketsPurchased = totalTicketsPurchased + {settingsModel.BattleTicketCount}," +
                                                          $"totalFiatConsumed = totalFiatConsumed + {settingsModel.BattleTicketPrice}",
                            conn);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    await conn.CloseAsync();
                    return Ok();
                }

                return BadRequest();
            }

            return BadRequest("Not enough balance");
        }

        [HttpGet]
        [ActionName("getTotalUsersJoined")]
        public async Task<IHttpActionResult> GetTotalUsersJoined()
        {
            try
            {
                var conn = _database.Connection;
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand($"select count(*) from postgres.public.usermodel", conn);
                int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                await conn.CloseAsync();
                return Content(HttpStatusCode.OK, count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        [ActionName("getTournamentSettings")]
        public async Task<IHttpActionResult> GetTournamentSettings()
        {
            try
            {
                var settingsModel = new SettingsModel();
                var conn = _database.Connection;
                await conn.OpenAsync();
                {
                    using var cmd = new NpgsqlCommand($"select * from postgres.public.settings", conn);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        settingsModel.Parse(reader);
                    }
                }
                await conn.CloseAsync();

                await conn.OpenAsync();
                {
                    using var cmd = new NpgsqlCommand($"select count(*) from postgres.public.usermodel", conn);
                    settingsModel.TotalUsersJoined = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                await conn.CloseAsync();
                return Content(HttpStatusCode.OK, settingsModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        [ActionName("didUserJoin")]
        public async Task<IHttpActionResult> DidUserJoin()
        {
            try
            {
                var validationToken = await ValidationHelper.Validate(Request.Headers.Authorization.Parameter);
                if (validationToken.success)
                {
                    if (await _database.CheckIfRecordExists("usermodel", "mm_address",
                            "'" + validationToken.mm_address + "'"))
                    {
                        return Ok();
                    }

                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return Unauthorized();
        }

        [HttpGet]
        [ActionName("getStats")]
        public async Task<IHttpActionResult> GetTournamentStats()
        {
            try
            {
                var userModel = await _database.Users.ToListAsync();
                userModel = userModel.OrderBy(o => o.EloRating).ToList();
                var totalBattles = 0;
                var totalWins = 0;
                var totalLosses = 0;
                var totalBattlePower = 0;
                int? totalTicketsPurchased = 0;
                int? totalFiatConsumed = 0;

                foreach (var user in userModel)
                {
                    totalWins += user.Wins;
                    totalLosses += user.Losses;
                    totalBattles += user.Wins + user.Losses;
                    totalBattlePower += user.BattlePower;
                }

                var divisor = (userModel.Count == 0) ? 1 : userModel.Count;
                
                var avgBattlePower = totalBattlePower / divisor;

                var conn = _database.Connection;
                var settingsModel = new SettingsModel();

                await conn.OpenAsync();
                {
                    using var cmd = new NpgsqlCommand($"select * from postgres.public.settings", conn);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        settingsModel.Parse(reader);
                    }
                }
                await conn.CloseAsync();

                await conn.OpenAsync();
                {
                    using var cmd = new NpgsqlCommand($"select * from postgres.public.stats", conn);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        totalTicketsPurchased = reader["totalTicketsPurchased"] as int?;
                        totalFiatConsumed = reader["totalFiatConsumed"] as int?;
                    }
                }
                await conn.CloseAsync();

                var returnValue = new
                {
                    Leaderboard = userModel.Take(10),
                    TotalBattles = totalBattles,
                    TotalWins = totalWins,
                    TotalLosses = totalLosses,
                    TotalBattlePower = totalBattlePower,
                    AverageBattlePower = avgBattlePower,
                    TotalTicketsPurchased = totalTicketsPurchased,
                    TotalFiatConsumed = totalFiatConsumed,
                    TotalUsers = userModel.Count,
                    CostPerTicket = settingsModel.BattleTicketPrice
                };
                return Content(HttpStatusCode.OK, returnValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}