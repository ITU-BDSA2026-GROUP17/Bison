RESULT=$(gh api graphql -F login='ITU-BDSA2026-GROUP17' -f query='
    query($login:String!) {
        organization(login:$login) {
            projectsV2(first:1) {
                nodes {
                    title,
                    field(name:"Iteration") {
                        ... on ProjectV2IterationField {
                            configuration {
                                iterations { title, id }
                            }
                        }
                    }
                }
            }
        }
    }
' | jq '.data.organization.projectsV2.nodes[0]')
echo "iter=$(echo $RESULT | jq -r '.field.configuration.iterations[0].id')"
echo "project=$(echo $RESULT | jq -r '.title')"
