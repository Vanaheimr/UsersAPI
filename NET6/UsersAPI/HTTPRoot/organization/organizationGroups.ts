///<reference path="../libs/date.format.ts" />

function StartOrganizationGroupSearch() {

    StartSearch<IOrganizationGroup>("/organizationGroups",
                                    "organizationGroup",
                                    "organizationGroups",
                                    "organization groups",
                                    (organizationGroup, organizationGroupDiv) => {
                                         CreateDiv(organizationGroupDiv, "id", organizationGroup["@id"]);
                                         organizationGroupDiv.appendChild(CreateI18NDiv(organizationGroup.name,         "name"));
                                         organizationGroupDiv.appendChild(CreateI18NDiv(organizationGroup.description,  "description"));
                                     },
                                     null,
                                     organizationGroup => "",
                                     null);

}
