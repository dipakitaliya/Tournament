using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CGRPG_Tournament.Models;
using CGRPG_TournamentLib.Contexts;
using CGRPG_TournamentLib.Helpers;
using CGRPG_TournamentLib.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Priority;

namespace CGRPG_TournamentTest
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UnitTest1
    {
        readonly TournamentContext _database = new TournamentContext();
        private readonly string _baseAddress = "http://localhost:9000/";
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _client;
        private readonly string bearerToken;
        
        protected class LoginData
        {
            public LoginData(string success)
            {
                Success = success;
            }

            public string Success { get; set; }
        }
        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
            _client = new HttpClient();
            var userdata = new List<KeyValuePair<string, string>>();
            userdata.Add(new KeyValuePair<string, string>("username", "idon@safename.io"));
            userdata.Add(new KeyValuePair<string, string>("password", "safename"));
            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.rpg.chainguardians.io/user/login") { Content = new FormUrlEncodedContent(userdata) };
            var response = _client.SendAsync(req).Result;
            LoginData d = JsonConvert.DeserializeObject<LoginData>(response.Content.ReadAsStringAsync().Result);
            bearerToken = d.Success;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", d.Success);
        }
        [Fact, Priority(00)]
        public Task TearDown()
        {
            //Arrange
            //Act
            _output.WriteLine("Deleted " + _database.RemoveAll("usermodel").Result + " rows from usermodel");
            _output.WriteLine("Deleted " + _database.RemoveAll("battles").Result + " rows from battles");
            return Task.CompletedTask;
            //Assert
        }

        [Fact, Priority(01)]
        public async Task Test01JoinNewlyJoined()
        {
            try
            {
                //Arrange
                JoinUserModel model = new JoinUserModel() { BattlePower = 1000, Units = "1219,1210,1219,21647,1029" };
                //Act
                var response = await _client.PostAsync(_baseAddress + "api/tournament/join",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                        "application/json"));
                //Assert
                Assert.Equal("OK", response.StatusCode.ToString());
            }
            catch (Exception e)
            {
                _output.WriteLine(e.Message);
                throw;
            }
            
        }

        [Fact, Priority(10)]
        public async Task Test02JoinAlreadyJoined()
        {
            //Arrange
            JoinUserModel model = new JoinUserModel() { BattlePower = 1000, Units = "1219,1210,1219,21647,1029" };
            //Act
            var response = await _client.PostAsync(_baseAddress + "api/tournament/join",
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                    "application/json"));
            //Assert
            Assert.Equal("BadRequest", response.StatusCode.ToString());
            Assert.Equal("{\"Message\":\"This user already joined\"}",
                response.Content.ReadAsStringAsync().Result);
        }

        [Fact, Priority(21)]
        public async Task Test04AttackPlayerAddTestingData()
        {
            //Arrange
            //Act
            _database.Users.Add(new UserModel { Username = "User00", BattlePower = 1301, EloRating = 1900.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a0", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User01", BattlePower = 1000, EloRating = 1800.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a1", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User02", BattlePower = 1000, EloRating = 1700.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a2", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User03", BattlePower = 1000, EloRating = 1600.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a3", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User04", BattlePower = 1000, EloRating = 1500.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a4", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User05", BattlePower = 1000, EloRating = 1400.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a5", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User06", BattlePower = 1000, EloRating = 1300.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a6", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User07", BattlePower = 1000, EloRating = 1200.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a7", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User08", BattlePower = 1000, EloRating = 1100.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a8", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User09", BattlePower = 1000, EloRating = 1000.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b0", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User10", BattlePower = 1000, EloRating = 900.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b1", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User11", BattlePower = 1000, EloRating = 800.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b2", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User12", BattlePower = 1000, EloRating = 700.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b3", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User13", BattlePower = 1000, EloRating = 600.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b4", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User14", BattlePower = 1000, EloRating = 500.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b5", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User15", BattlePower = 1000, EloRating = 400.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b6", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User16", BattlePower = 1000, EloRating = 300.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b7", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User17", BattlePower = 1000, EloRating = 200.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b8", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User18", BattlePower = 1000, EloRating = 100.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3b9", Units = "1,2,3,4,5" });
            _database.Users.Add(new UserModel { Username = "User19", BattlePower = 1000, EloRating = 10.0f, BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3c0", Units = "1,2,3,4,5" });

            await _database.SaveChangesAsync();
            //Assert
        }
        
        [Fact, Priority(22)]
        public async Task Test05StartBattle()
        {
            //Arrange
            StartBattleModel model = new StartBattleModel();
            model.MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a9";
            model.MetaMaskAddressOpponent = "0xab83bd5169f58e753d291223dcaba4f7644ad3a8";
            //Act
            _output.WriteLine(JsonConvert.SerializeObject(model));
            var response = await _client.PostAsync(_baseAddress + "api/tournament/startBattle", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            
            //Assert
            var list = _database.Battles.ToListAsync().Result;
            Assert.Equal(response.Content.ReadAsStringAsync().Result.Trim().Trim('"'), list.OrderByDescending(b => b.OnCreated).FirstOrDefault()!.BattleId.ToString());
        }
        
        [Fact, Priority(23)]
        public async Task Test06AttackPlayerSuccess()
        {
            //Arrange
            AttackPlayerModel model = new AttackPlayerModel();
            model.BattleId = _database.Battles.ToListAsync().Result.OrderByDescending(b => b.OnCreated).FirstOrDefault()!.BattleId;
            model.MetaMaskAddress = "0xab83bd5169f58e753d291223dcaba4f7644ad3a9";
            model.MetaMaskAddressOpponent = "0xab83bd5169f58e753d291223dcaba4f7644ad3a0";
            model.DidIWin = true;
            //Act
            _output.WriteLine(JsonConvert.SerializeObject(model));
            var response = await _client.PostAsync(_baseAddress + "api/tournament/attackPlayer",
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            //Assert
            Assert.Equal("OK", response.StatusCode.ToString());
        }

        [Fact, Priority(30)]
        public async Task Test07GetUser()
        {
            //Arrange
            //Act
            var response = await _client.GetAsync(_baseAddress + "api/tournament/getUser");

            //Assert
            Assert.Equal("OK", response.StatusCode.ToString());
            //Assert.Equal();
            //TODO: finish this after the call
            
        }

        [Fact, Priority(40)]
        public async Task Test08GetUserBattleTickets()
        {
            //Arrange
            //Act
            var response = await _client.GetAsync(_baseAddress + "api/tournament/getUserBattleTickets");
            //Assert
            Assert.Equal("OK", response.StatusCode.ToString());
            Assert.Equal("4", response.Content.ReadAsStringAsync().Result.Trim().Trim('"')); //one battle happened already
        }

        [Fact, Priority(50)]
        public async Task Test09GetListOfUsers()
        {
            //Arrange
            //Act
            var response = await _client.GetAsync(_baseAddress + "api/tournament/listOpponents");
            //Assert
            var users = JsonConvert.DeserializeObject<List<UserModel>>(response.Content.ReadAsStringAsync().Result);
            Assert.Equal(11, users.Count);
            
            //Top 3
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a0", users[0].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a1", users[1].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a2", users[2].MetaMaskAddress);
            
            //3 above
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a6", users[3].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a7", users[4].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a8", users[5].MetaMaskAddress);
            
            //You
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3a9", users[6].MetaMaskAddress);
            
            //4 below
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3b0", users[7].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3b1", users[8].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3b2", users[9].MetaMaskAddress);
            Assert.Equal("0xab83bd5169f58e753d291223dcaba4f7644ad3b3", users[10].MetaMaskAddress);
        }

        [Fact, Priority(60)]
        public async Task Test10AddFunds()
        {
            //Arrange
            //Act
            var newBalance = await ChilliConnectHelper.SetBalance(bearerToken, 0, 1000);
            var balance = await ChilliConnectHelper.GetBalance(bearerToken);
            //Assert
            Assert.Equal(1000, balance.Balances[0].Balance);
            Assert.Equal(newBalance.Balances[0].Balance, balance.Balances[0].Balance);
        }
        
        [Fact, Priority(70)]
        public async Task Test11UpdateUserBattleTicketsSuccess()
        {
            //Arrange
            //Act
            _output.WriteLine(JsonConvert.SerializeObject("cgdevs"));
            var response = await _client.PostAsync(_baseAddress + "api/tournament/updateUserBattleTickets", 
                new StringContent(JsonConvert.SerializeObject("cgdevs"), Encoding.UTF8, "application/json"));
            var ticketsResponse = await _client.GetAsync(_baseAddress + "api/tournament/getUserBattleTickets");
            //Assert
            Assert.Equal("OK", response.StatusCode.ToString());
            Assert.Equal("OK", ticketsResponse.StatusCode.ToString());
            Assert.Equal("9", ticketsResponse.Content.ReadAsStringAsync().Result);
        }
        
        [Fact, Priority(80)]
        public async Task Test12UpdateUserBattleTicketsFailure()
        {
            //Arrange
            //Act
            var response = await _client.PostAsync(_baseAddress + "api/tournament/updateUserBattleTickets", 
                new StringContent(JsonConvert.SerializeObject("cgdevs"), Encoding.UTF8, "application/json"));
            //Assert
            Assert.Equal("BadRequest", response.StatusCode.ToString());
            Assert.Equal("{\"Message\":\"Not enough balance\"}",
                response.Content.ReadAsStringAsync().Result);
        }

        [Fact, Priority(90)]
        public async Task Test13GenerateALotOfUsers()
        {
            var rand = new Random();
            for (int i = 20; i < 5000; ++i)
            {
                _database.Users.Add(new UserModel { Username = $"User{i}", BattlePower = rand.Next(1000, 10000), EloRating = 500.0f + rand.Next(0, 7000), BattleTickets = 5, Timestamp = DateTime.Now, MetaMaskAddress = String.Format( "{0:X40}", i), Units = "1,2,3,4,5" });
            }
            await _database.SaveChangesAsync();
        }
    }
}