using MDD4All.MongoDB.DataAccess.Generic;
using MDD4All.SpecIF.DataModels.RightsManagement;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MDD4All.SpecIF.DataProvider.MongoDB.Setup
{
    public class AdminDatabaseInitializer
    {
        private string _connectionString;

        private MongoDBDataAccessor<ApplicationRole> _roleMongoDbAccessor;
        private MongoDBDataAccessor<ApplicationUser> _userMongoDbAccessor;
        private MongoDBDataAccessor<JwtConfiguration> _jwtMongoDbAccessor;

        public AdminDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitalizeAdminData()
        {
            if (!AdminDbExists())
            {
                CreateAdminUserAndRoles(_connectionString);
                CreateJwtConfiguration(_connectionString);
            }
        }

        public bool AdminDbExists()
        {
            bool result = true;

            MongoClient mongoClient = new MongoClient(_connectionString);

            List<string> databaseNames = mongoClient.ListDatabaseNames().ToList();

            if (!databaseNames.Contains("specifAdmin"))
            {
                result = false;
            }

            return result;
        }

        public void CreateAdminUserAndRoles(string connectionString)
        {

            _roleMongoDbAccessor = new MongoDBDataAccessor<ApplicationRole>(connectionString, "specifAdmin");
            _userMongoDbAccessor = new MongoDBDataAccessor<ApplicationUser>(connectionString, "specifAdmin");

            _roleMongoDbAccessor.Add(new ApplicationRole
            {
                Id = "ROLE-Admin",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });

            _roleMongoDbAccessor.Add(new ApplicationRole
            {
                Id = "ROLE-Editor",
                Name = "Editor",
                NormalizedName = "EDITOR"
            });
            _roleMongoDbAccessor.Add(new ApplicationRole
            {
                Id = "ROLE-Reader",
                Name = "READER",
                NormalizedName = "READER"
            });

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();

            ApplicationUser adminUser = new ApplicationUser
            {
                Id = "User-Admin",
                NormalizedUserName = "ADMIN",
                UserName = "admin",
                ApiKey = "F109BE57-BB60-42B5-A6E8-30DBDA7BEE2F",
                Roles = new List<string>
                {
                    "Administrator"
                }
            };

            string hash = passwordHasher.HashPassword(adminUser, "password");

            adminUser.PasswordHash = hash;

            _userMongoDbAccessor.Add(adminUser);

            ApplicationUser editorUser = new ApplicationUser
            {
                Id = "User-Editor",
                NormalizedUserName = "Editor",
                UserName = "editor",
                ApiKey = "5824E518-C2C6-455A-9447-8A8CA029E40C",
                Roles = new List<string>
                {

                    "Editor"
                }
            };

            string hashEditor = passwordHasher.HashPassword(editorUser, "password");

            editorUser.PasswordHash = hashEditor;

            _userMongoDbAccessor.Add(editorUser);

            ApplicationUser readerUser = new ApplicationUser
            {
                Id = "User-Reader",
                NormalizedUserName = "Reader",
                UserName = "reader",
                ApiKey = "5824E518-A1D5-455A-9447-8A8CA029AB37",
                Roles = new List<string>
                {

                    "Reader"
                }
            };

            string hashReader = passwordHasher.HashPassword(readerUser, "password");

            readerUser.PasswordHash = hashReader;

            _userMongoDbAccessor.Add(readerUser);
        }

        public void CreateJwtConfiguration(string connectionString)
        {
            _jwtMongoDbAccessor = new MongoDBDataAccessor<JwtConfiguration>(connectionString, "specifAdmin");

            JwtConfiguration jwtConfiguration = new JwtConfiguration()
            {
                Secret = Guid.NewGuid().ToString()
            };

            _jwtMongoDbAccessor.Add(jwtConfiguration);
        }
    }
}
