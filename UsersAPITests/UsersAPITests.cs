/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://github.com/Vanaheimr/UsersAPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using social.OpenData.UsersAPI.Notifications;

#endregion

namespace social.OpenData.UsersAPI.tests
{

    /// <summary>
    /// Users API tests.
    /// </summary>
    [TestFixture]
    public class UsersAPITests
    {

        #region Start/Stop Users API

        private UsersAPI    usersAPI;
        private NullMailer  nullMailer;

        [OneTimeSetUp]
        public void SetupOnce()
        {

        }

        [SetUp]
        public void SetupEachTest()
        {

            usersAPI = new UsersAPI(
                           HTTPPort:               IPPort.Parse(81),
                           APIRobotEMailAddress:   new EMailAddress(
                                                       "Users API Unit Tests",
                                                       SimpleEMailAddress.Parse("robot@opendata.social")
                                                   ),
                           AdminOrganizationId:    Organization_Id.Parse("admins"),
                           APISMTPClient:          new NullMailer(),
                           Autostart:              true
                       );

            nullMailer = usersAPI.APISMTPClient as NullMailer;

            usersAPI.AddOrganization(new Organization(
                                         Organization_Id.Parse ("admins"),
                                         I18NString.     Create(Languages.en, "Admins")
                                    )).Wait();

            #region /

            //_HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                              HTTPMethod.GET,
            //                              HTTPPath.Root,
            //                              HTTPDelegate: Request => Task.FromResult(
            //                                                            new HTTPResponse.Builder(Request) {
            //                                                                HTTPStatusCode             = HTTPStatusCode.OK,
            //                                                                Server                     = "Test Server",
            //                                                                Date                       = DateTime.UtcNow,
            //                                                                AccessControlAllowOrigin   = "*",
            //                                                                AccessControlAllowMethods  = "GET",
            //                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
            //                                                                ContentType                = HTTPContentType.TEXT_UTF8,
            //                                                                Content                    = "Hello World!".ToUTF8Bytes(),
            //                                                                Connection                 = "close"
            //                                                            }.AsImmutable));

            #endregion

        }

        [TearDown]
        public void ShutdownEachTest()
        {
            usersAPI.Shutdown();
        }

        [OneTimeTearDown]
        public void ShutdownOnce()
        {

        }

        #endregion


        #region UsersAPI_Test01()

        [Test]
        public async Task UsersAPI_Test01()
        {

            Assert.AreEqual(0, usersAPI.Users.        Count());
            Assert.AreEqual(2, usersAPI.Organizations.Count());

            Assert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("NoOwner")));
            Assert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("admins")));


            var result01a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiAdmin01"),
                                                       "API Admin 01",
                                                       SimpleEMailAddress.Parse("apiAdmin01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            Assert.IsNotNull(result01a);
            Assert.IsTrue   (result01a.IsSuccess);
            Assert.AreEqual (1, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result01a.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result01b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiAdmin02"),
                                                       "API Admin 02",
                                                       SimpleEMailAddress.Parse("apiAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            Assert.IsNotNull(result01b);
            Assert.IsTrue   (result01b.IsSuccess);
            Assert.AreEqual (2, usersAPI.Users.Count());


            var result01c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiMember01"),
                                                       "API Member 01",
                                                       SimpleEMailAddress.Parse("apiMember01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            Assert.IsNotNull(result01c);
            Assert.IsTrue   (result01c.IsSuccess);
            Assert.AreEqual (3, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result01c.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result01d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiMember02"),
                                                       "API Member 02",
                                                       SimpleEMailAddress.Parse("apiMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            Assert.IsNotNull(result01d);
            Assert.IsTrue   (result01d.IsSuccess);
            Assert.AreEqual (4, usersAPI.Users.Count());


            #region Setup FirstOrg

            var result03 = await usersAPI.AddOrganization(new Organization(
                                                              Organization_Id.Parse("firstOrg"),
                                                              I18NString.Create(Languages.en, "First Organization")
                                                          ),
                                                          ParentOrganization: result01a.Organization);

            Assert.AreEqual(3, usersAPI.Organizations.Count());
            Assert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("firstOrg")));

            var IsChildOrganizationEdge1 = result03.Organization.Organization2OrganizationOutEdges.FirstOrDefault();

            Assert.IsNotNull(IsChildOrganizationEdge1);
            Assert.AreEqual(Organization_Id.Parse("firstOrg"), IsChildOrganizationEdge1.Source.Id);
            Assert.AreEqual(usersAPI.AdminOrganizationId,      IsChildOrganizationEdge1.Target.Id);


            var result04a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgAdmin01"),
                                                       "First Org Admin 01",
                                                       SimpleEMailAddress.Parse("firstOrgAdmin01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   result03.Organization);

            Assert.IsNotNull(result04a);
            Assert.IsTrue   (result04a.IsSuccess);
            Assert.AreEqual (5, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result04a.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result04b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgAdmin02"),
                                                       "First Org Admin 02",
                                                       SimpleEMailAddress.Parse("firstOrgAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   result03.Organization);

            Assert.IsNotNull(result04b);
            Assert.IsTrue   (result04b.IsSuccess);
            Assert.AreEqual (6, usersAPI.Users.Count());


            var result04c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgMember01"),
                                                       "First Org Member 01",
                                                       SimpleEMailAddress.Parse("firstOrgMember01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   result03.Organization);


            Assert.IsNotNull(result04c);
            Assert.IsTrue   (result04c.IsSuccess);
            Assert.AreEqual (7, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result04c.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result04d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgMember02"),
                                                       "First Org Member 02",
                                                       SimpleEMailAddress.Parse("firstOrgMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   result03.Organization);

            Assert.IsNotNull(result04d);
            Assert.IsTrue   (result04d.IsSuccess);
            Assert.AreEqual (8, usersAPI.Users.Count());

            #endregion

            #region Setup SecondOrg

            var result13 = await usersAPI.AddOrganization(new Organization(
                                                              Organization_Id.Parse("secondOrg"),
                                                              I18NString.Create(Languages.en, "Second Organization")
                                                          ),
                                                          ParentOrganization: result03.Organization);

            Assert.AreEqual(4, usersAPI.Organizations.Count());
            Assert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("secondOrg")));

            var IsChildOrganizationEdge2 = result13.Organization.Organization2OrganizationOutEdges.FirstOrDefault();

            Assert.IsNotNull(IsChildOrganizationEdge2);
            Assert.AreEqual(Organization_Id.Parse("secondOrg"), IsChildOrganizationEdge2.Source.Id);
            Assert.AreEqual(Organization_Id.Parse("firstOrg"),  IsChildOrganizationEdge2.Target.Id);


            var result14a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgAdmin01"),
                                                       "Second Org Admin 01",
                                                       SimpleEMailAddress.Parse("secondOrgAdmin01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   result13.Organization);

            Assert.IsNotNull(result14a);
            Assert.IsTrue   (result14a.IsSuccess);
            Assert.AreEqual (9, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result14a.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result14b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgAdmin02"),
                                                       "Second Org Admin 02",
                                                       SimpleEMailAddress.Parse("secondOrgAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsAdmin,
                                                   result13.Organization);

            Assert.IsNotNull(result14b);
            Assert.IsTrue   (result14b.IsSuccess);
            Assert.AreEqual (10, usersAPI.Users.Count());


            var result14c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgMember01"),
                                                       "Second Org Member 01",
                                                       SimpleEMailAddress.Parse("secondOrgMember01@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   result13.Organization);


            Assert.IsNotNull(result14c);
            Assert.IsTrue   (result14c.IsSuccess);
            Assert.AreEqual (11, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result14c.User,
                                                new NotificationMessageType[] {
                                                    UsersAPI.addUser_MessageType,
                                                    UsersAPI.updateUser_MessageType,
                                                    UsersAPI.deleteUser_MessageType,

                                                    UsersAPI.addUserToOrganization_MessageType,
                                                    UsersAPI.removeUserFromOrganization_MessageType,

                                                    UsersAPI.addOrganization_MessageType,
                                                    UsersAPI.updateOrganization_MessageType,
                                                    UsersAPI.deleteOrganization_MessageType,

                                                    UsersAPI.linkOrganizations_MessageType,
                                                    UsersAPI.unlinkOrganizations_MessageType
                                                });


            var result14d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgMember02"),
                                                       "Second Org Member 02",
                                                       SimpleEMailAddress.Parse("secondOrgMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeTypes.IsMember,
                                                   result13.Organization);

            Assert.IsNotNull(result14d);
            Assert.IsTrue   (result14d.IsSuccess);
            Assert.AreEqual (12, usersAPI.Users.Count());

            #endregion



            var maxEMailSubjectLength  = nullMailer.EMails.Max   (emailEnvelope => emailEnvelope.Mail.Subject.Length);
            var allEMailNotifications  = nullMailer.EMails.Select(emailEnvelope => emailEnvelope.Mail.Subject.PadRight(maxEMailSubjectLength + 2) + " => " + emailEnvelope.RcptTo.Select(email => email.Address).AggregateWith(", ")).ToArray();
            var eMailOverview          = allEMailNotifications.AggregateWith(Environment.NewLine);

            // User 'API Admin 02' was successfully created.                                            => apiAdmin01@test.local
            // User 'API Admin 02' was added to organization 'Admins' as admin.                         => apiAdmin01@test.local
            // User 'API Member 01' was successfully created.                                           => apiAdmin01@test.local
            // User 'API Member 01' was added to organization 'Admins' as member.                       => apiAdmin01@test.local
            // User 'API Member 02' was successfully created.                                           => apiAdmin01@test.local, apiMember01@test.local
            // User 'API Member 02' was added to organization 'Admins' as member.                       => apiAdmin01@test.local, apiMember01@test.local
            //
            // Organization 'First Organization' was successfully created.                              => apiAdmin01@test.local, apiMember01@test.local
            // Organization 'First Organization' was linked to organization 'Admins'.                   => apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Admin 01' was successfully created.                                      => apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Admin 01' was added to organization 'First Organization' as admin.       => apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Admin 02' was successfully created.                                      => firstOrgAdmin01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Admin 02' was added to organization 'First Organization' as admin.       => firstOrgAdmin01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Member 01' was successfully created.                                     => firstOrgAdmin01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Member 01' was added to organization 'First Organization' as member.     => firstOrgAdmin01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Member 02' was successfully created.                                     => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'First Org Member 02' was added to organization 'First Organization' as member.     => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            //
            // Organization 'Second Organization' was successfully created.                             => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // Organization 'Second Organization' was linked to organization 'First Organization'.      => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Admin 01' was successfully created.                                     => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Admin 01' was added to organization 'Second Organization' as admin.     => firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Admin 02' was successfully created.                                     => secondOrgAdmin01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Admin 02' was added to organization 'Second Organization' as admin.     => secondOrgAdmin01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Member 01' was successfully created.                                    => secondOrgAdmin01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Member 01' was added to organization 'Second Organization' as member.   => secondOrgAdmin01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Member 02' was successfully created.                                    => secondOrgAdmin01@test.local, secondOrgMember01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local
            // User 'Second Org Member 02' was added to organization 'Second Organization' as member.   => secondOrgAdmin01@test.local, secondOrgMember01@test.local, firstOrgAdmin01@test.local, firstOrgMember01@test.local, apiAdmin01@test.local, apiMember01@test.local

        }

        #endregion


    }

}
