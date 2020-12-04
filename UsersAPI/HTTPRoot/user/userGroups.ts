/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/search.ts" />

function StartUserGroupSearch() {

    StartSearch<IUserGroup>("/userGroups",
                            "userGroup",
                            "userGroups",
                            "user groups",
                            (userGroup, userGroupDiv) => {

                                //CreateDiv(userGroupDiv, "id",    userGroup["@id"]);
                                userGroupDiv.appendChild(CreateI18NDiv(userGroup.name,        "name"));
                                userGroupDiv.appendChild(CreateI18NDiv(userGroup.description, "description"));

                                if (userGroup.isMember != undefined) {
                                    const statusDiv      = userGroupDiv.appendChild(document.createElement('div'));
                                    statusDiv.className  = "isMember";
                                    statusDiv.innerHTML  = userGroup.isMember.length + " members";
                                }

                            },
                            null,
                            userGroup => "",
                            null);

}
