///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartOrganizationGroupSearch() {

    StartSearch<IOrganizationGroup>("/organizationGroup",
                                    "organizationGroup",
                                    "organizationGroups",
                                    (organizationGroup, organizationGroupDiv) => {
                                         CreateDiv(organizationGroupDiv, "id", organizationGroup["@id"]);
                                         organizationGroupDiv.appendChild(CreateI18NDiv(organizationGroup.name,         "name"));
                                         organizationGroupDiv.appendChild(CreateI18NDiv(organizationGroup.description,  "description"));
                                     },
                                     null,
                                     organizationGroup => "",
                                     null);

}
