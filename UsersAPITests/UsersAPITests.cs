/*
 * Copyright (c) 2014-2023, Achim Friedland <achim.friedland@graphdefined.com>
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

using NUnit.Framework;
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP.Notifications;

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
        private NullSMSAPI  nullSMSClient;

        [OneTimeSetUp]
        public void SetupOnce()
        {

        }

        [SetUp]
        public void SetupEachTest()
        {

            var folder = "UsersAPI_NotificationTests";

            // C:\Users\...\AppData\Local\Temp\UsersAPI_NotificationTests
            try
            {
                Directory.Delete(folder, true);
            }
            catch { }

            usersAPI = new UsersAPI(
                           ExternalDNSName:        "example.cloud",
                           HTTPServerPort:               IPPort.Parse(81),
                           APIRobotEMailAddress:   new EMailAddress(
                                                       "Users API Unit Tests",
                                                       SimpleEMailAddress.Parse("robot@opendata.social")
                                                   ),
                           AdminOrganizationId:    Organization_Id.Parse("admins"),
                           SMTPClient:             new NullMailer(),
                           SMSClient:              new NullSMSAPI(),
                           LoggingPath:            folder,
                           AutoStart:              true
                       );;

            nullMailer     = usersAPI.SMTPClient as NullMailer;
            nullSMSClient  = usersAPI.SMSClient  as NullSMSAPI;

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
            //                                                                AccessControlAllowMethods  = new[] { "GET" },
            //                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
            //                                                                ContentType                = HTTPContentType.Text.TEXT_UTF8,
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

            ClassicAssert.AreEqual(0, usersAPI.Users.        Count());
            ClassicAssert.AreEqual(2, usersAPI.Organizations.Count());

            ClassicAssert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("NoOwner")));
            ClassicAssert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("admins")));


            var result01a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiAdmin01"),
                                                       "API Admin 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("apiAdmin01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 111111")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            ClassicAssert.IsNotNull(result01a);
            ClassicAssert.IsTrue   (result01a.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result01a.User);
            ClassicAssert.AreEqual (1, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result01a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result01a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result01b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiAdmin02"),
                                                       "API Admin 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("apiAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            ClassicAssert.IsNotNull(result01b);
            ClassicAssert.IsTrue   (result01b.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result01b.User);
            ClassicAssert.AreEqual (2, usersAPI.Users.Count());


            var result01c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiMember01"),
                                                       "API Member 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("apiMember01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 222222")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            ClassicAssert.IsNotNull(result01c);
            ClassicAssert.IsTrue   (result01c.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result01c.User);
            ClassicAssert.AreEqual (3, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result01c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result01c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result01d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("apiMember02"),
                                                       "API Member 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("apiMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   usersAPI.GetOrganization(Organization_Id.Parse("admins")));

            ClassicAssert.IsNotNull(result01d);
            ClassicAssert.IsTrue   (result01d.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result01d.User);
            ClassicAssert.AreEqual (4, usersAPI.Users.Count());


            #region Setup FirstOrg

            var result03 = await usersAPI.AddOrganization(new Organization(
                                                              Organization_Id.Parse("firstOrg"),
                                                              I18NString.Create(Languages.en, "First Organization")
                                                          ),
                                                          ParentOrganization: result01a.Organization);

            ClassicAssert.AreEqual(3, usersAPI.Organizations.Count());
            ClassicAssert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("firstOrg")));

            var IsChildOrganizationEdge1 = result03.Organization.Organization2OrganizationOutEdges.FirstOrDefault();

            ClassicAssert.IsNotNull(IsChildOrganizationEdge1);
            ClassicAssert.AreEqual(Organization_Id.Parse("firstOrg"), IsChildOrganizationEdge1.Source.Id);
            ClassicAssert.AreEqual(usersAPI.AdminOrganizationId,      IsChildOrganizationEdge1.Target.Id);


            var result04a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgAdmin01"),
                                                       "First Org Admin 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("firstOrgAdmin01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 333333")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   result03.Organization);

            ClassicAssert.IsNotNull(result04a);
            ClassicAssert.IsTrue   (result04a.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result04a.User);
            ClassicAssert.AreEqual (5, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result04a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result04a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result04b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgAdmin02"),
                                                       "First Org Admin 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("firstOrgAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   result03.Organization);

            ClassicAssert.IsNotNull(result04b);
            ClassicAssert.IsTrue   (result04b.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result04b.User);
            ClassicAssert.AreEqual (6, usersAPI.Users.Count());


            var result04c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgMember01"),
                                                       "First Org Member 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("firstOrgMember01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 444444")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   result03.Organization);


            ClassicAssert.IsNotNull(result04c);
            ClassicAssert.IsTrue   (result04c.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result04c.User);
            ClassicAssert.AreEqual (7, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result04c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result04c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result04d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("firstOrgMember02"),
                                                       "First Org Member 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("firstOrgMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   result03.Organization);

            ClassicAssert.IsNotNull(result04d);
            ClassicAssert.IsTrue   (result04d.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result04d.User);
            ClassicAssert.AreEqual (8, usersAPI.Users.Count());

            #endregion

            #region Setup SecondOrg

            var result13 = await usersAPI.AddOrganization(new Organization(
                                                              Organization_Id.Parse("secondOrg"),
                                                              I18NString.Create(Languages.en, "Second Organization")
                                                          ),
                                                          ParentOrganization: result03.Organization);

            ClassicAssert.AreEqual(4, usersAPI.Organizations.Count());
            ClassicAssert.IsTrue  (usersAPI.OrganizationExists(Organization_Id.Parse("secondOrg")));

            var IsChildOrganizationEdge2 = result13.Organization.Organization2OrganizationOutEdges.FirstOrDefault();

            ClassicAssert.IsNotNull(IsChildOrganizationEdge2);
            ClassicAssert.AreEqual(Organization_Id.Parse("secondOrg"), IsChildOrganizationEdge2.Source.Id);
            ClassicAssert.AreEqual(Organization_Id.Parse("firstOrg"),  IsChildOrganizationEdge2.Target.Id);


            var result14a = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgAdmin01"),
                                                       "Second Org Admin 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("secondOrgAdmin01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 555555")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   result13.Organization);

            ClassicAssert.IsNotNull(result14a);
            ClassicAssert.IsTrue   (result14a.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result14a.User);
            ClassicAssert.AreEqual (9, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result14a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result14a.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result14b = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgAdmin02"),
                                                       "Second Org Admin 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("secondOrgAdmin02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsAdmin,
                                                   result13.Organization);

            ClassicAssert.IsNotNull(result14b);
            ClassicAssert.IsTrue   (result14b.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result14b.User);
            ClassicAssert.AreEqual (10, usersAPI.Users.Count());


            var result14c = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgMember01"),
                                                       "Second Org Member 01".ToI18NString(),
                                                       SimpleEMailAddress.Parse("secondOrgMember01@test.local"),
                                                       MobilePhone: PhoneNumber.Parse("+49 170 666666")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   result13.Organization);


            ClassicAssert.IsNotNull(result14c);
            ClassicAssert.IsTrue   (result14c.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result14c.User);
            ClassicAssert.AreEqual (11, usersAPI.Users.Count());

            await usersAPI.AddEMailNotification(result14c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });

            await usersAPI.AddSMSNotification  (result14c.User!,
                                                new[] {
                                                    HTTPExtAPI.addUser_MessageType,
                                                    HTTPExtAPI.updateUser_MessageType,
                                                    HTTPExtAPI.deleteUser_MessageType,

                                                    HTTPExtAPI.addUserToOrganization_MessageType,
                                                    HTTPExtAPI.removeUserFromOrganization_MessageType,

                                                    HTTPExtAPI.addOrganization_MessageType,
                                                    HTTPExtAPI.updateOrganization_MessageType,
                                                    HTTPExtAPI.deleteOrganization_MessageType,

                                                    HTTPExtAPI.linkOrganizations_MessageType,
                                                    HTTPExtAPI.unlinkOrganizations_MessageType
                                                });


            var result14d = await usersAPI.AddUser(new User(
                                                       User_Id.Parse("secondOrgMember02"),
                                                       "Second Org Member 02".ToI18NString(),
                                                       SimpleEMailAddress.Parse("secondOrgMember02@test.local")
                                                   ),
                                                   User2OrganizationEdgeLabel.IsMember,
                                                   result13.Organization);

            ClassicAssert.IsNotNull(result14d);
            ClassicAssert.IsTrue   (result14d.Result == CommandResult.Success);
            ClassicAssert.IsNotNull(result14d.User);
            ClassicAssert.AreEqual (12, usersAPI.Users.Count());

            #endregion


            ClassicAssert.IsTrue(nullMailer.EMailEnvelops.Any(), "Not a single notification e-mail was sent!");

            var maxEMailSubjectLength  = nullMailer.EMailEnvelops.Max   (emailEnvelope => emailEnvelope.Mail.Subject.Length);
            var allEMailNotifications  = nullMailer.EMailEnvelops.Select(emailEnvelope => emailEnvelope.Mail.Subject.PadRight(maxEMailSubjectLength + 2) + " => " + emailEnvelope.RcptTo.Select(email => email.Address).OrderBy(_ => _).AggregateWith(", ")).ToArray();
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


            ClassicAssert.IsTrue(nullSMSClient.SMSs.Any(), "Not a single notification SMS was sent!");

            var maxSMSMessageLength  = nullSMSClient.SMSs.Max   (sms => sms.Text.Length);
            var allSMSNotifications  = nullSMSClient.SMSs.Select(sms => sms.Text.PadRight(maxSMSMessageLength + 2) + " => " + sms.Receivers.OrderBy(_ =>_).AggregateWith(", ")).ToArray();
            var smsOverview          = allSMSNotifications.AggregateWith(Environment.NewLine);

            // |-- 160 characters --------------------------------------------------------------------------------------------------------------------------------------------|
            // User 'API Admin 02' was successfully added. https://example.cloud/users/apiAdmin02                           => +49 170 111111
            // User 'API Admin 02' was added to organization 'Admins' as admin.                                             => +49 170 111111
            // User 'API Member 01' was successfully added. https://example.cloud/users/apiMember01                         => +49 170 111111
            // User 'API Member 01' was added to organization 'Admins' as member.                                           => +49 170 111111
            // User 'API Member 02' was successfully added. https://example.cloud/users/apiMember02                         => +49 170 111111, +49 170 222222
            // User 'API Member 02' was added to organization 'Admins' as member.                                           => +49 170 111111, +49 170 222222
            //
            // Organization 'First Organization' was successfully created. https://example.cloud/organizations/firstOrg     => +49 170 111111, +49 170 222222
            // Organization 'First Organization' was linked to organization 'Admins'.                                       => +49 170 111111, +49 170 222222
            // User 'First Org Admin 01' was successfully added. https://example.cloud/users/firstOrgAdmin01                => +49 170 111111, +49 170 222222
            // User 'First Org Admin 01' was added to organization 'First Organization' as admin.                           => +49 170 111111, +49 170 222222
            // User 'First Org Admin 02' was successfully added. https://example.cloud/users/firstOrgAdmin02                => +49 170 111111, +49 170 222222, +49 170 333333
            // User 'First Org Admin 02' was added to organization 'First Organization' as admin.                           => +49 170 111111, +49 170 222222, +49 170 333333
            // User 'First Org Member 01' was successfully added. https://example.cloud/users/firstOrgMember01              => +49 170 111111, +49 170 222222, +49 170 333333
            // User 'First Org Member 01' was added to organization 'First Organization' as member.                         => +49 170 111111, +49 170 222222, +49 170 333333
            // User 'First Org Member 02' was successfully added. https://example.cloud/users/firstOrgMember02              => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            // User 'First Org Member 02' was added to organization 'First Organization' as member.                         => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            //
            // Organization 'Second Organization' was successfully created. https://example.cloud/organizations/secondOrg   => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            // Organization 'Second Organization' was linked to organization 'First Organization'.                          => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            // User 'Second Org Admin 01' was successfully added. https://example.cloud/users/secondOrgAdmin01              => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            // User 'Second Org Admin 01' was added to organization 'Second Organization' as admin.                         => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444
            // User 'Second Org Admin 02' was successfully added. https://example.cloud/users/secondOrgAdmin02              => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555
            // User 'Second Org Admin 02' was added to organization 'Second Organization' as admin.                         => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555
            // User 'Second Org Member 01' was successfully added. https://example.cloud/users/secondOrgMember01            => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555
            // User 'Second Org Member 01' was added to organization 'Second Organization' as member.                       => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555
            // User 'Second Org Member 02' was successfully added. https://example.cloud/users/secondOrgMember02            => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555, +49 170 666666
            // User 'Second Org Member 02' was added to organization 'Second Organization' as member.                       => +49 170 111111, +49 170 222222, +49 170 333333, +49 170 444444, +49 170 555555, +49 170 666666

        }

        #endregion


    }

}
