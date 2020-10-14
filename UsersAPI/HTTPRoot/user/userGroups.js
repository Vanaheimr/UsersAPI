/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/search.ts" />
function StartUserGroupSearch() {
    StartSearch("/userGroups", "userGroup", "userGroups", "user groups", (userGroup, userGroupDiv) => {
        CreateDiv(userGroupDiv, "id", userGroup["@id"]);
        CreateDiv(userGroupDiv, "name", CreateI18NDiv(userGroup.name, "name"));
        CreateDiv(userGroupDiv, null, CreateI18NDiv(userGroup.description, "description"));
        //CreateDiv(userDiv, "email", "<a href=\"" + user.email + "\">" + user.email + "</a>");
    }, null, userGroup => "", null);
}
//# sourceMappingURL=userGroups.js.map