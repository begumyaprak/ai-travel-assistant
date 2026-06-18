import json
import urllib.request

SEARCH_ENDPOINT = "YOUR_AZURE_SEARCH_ENDPOINT"
SEARCH_API_KEY = "YOUR_AZURE_SEARCH_API_KEY"
INDEX_NAME = "travel-documents"
API_VERSION = "2023-11-01"

index_definition = {
    "name": INDEX_NAME,
    "fields": [
        {"name": "id", "type": "Edm.String", "key": True, "filterable": True},
        {"name": "documentId", "type": "Edm.String", "filterable": True},
        {"name": "fileName", "type": "Edm.String", "searchable": True},
        {"name": "content", "type": "Edm.String", "searchable": True},
        {"name": "chunkIndex", "type": "Edm.Int32"},
        {"name": "pageReference", "type": "Edm.String", "filterable": True},
        {"name": "category", "type": "Edm.String", "filterable": True},
        {"name": "destination", "type": "Edm.String", "filterable": True},
        {
            "name": "contentVector",
            "type": "Collection(Edm.Single)",
            "searchable": True,
            "dimensions": 1536,
            "vectorSearchProfile": "vector-profile"
        }
    ],
    "vectorSearch": {
        "algorithms": [
            {"name": "hnsw-algorithm", "kind": "hnsw", "hnswParameters": {"metric": "cosine"}}
        ],
        "profiles": [
            {"name": "vector-profile", "algorithm": "hnsw-algorithm"}
        ]
    },
    "semantic": {
        "configurations": [
            {
                "name": "travel-semantic-config",
                "prioritizedFields": {
                    "prioritizedContentFields": [{"fieldName": "content"}],
                    "prioritizedKeywordsFields": [{"fieldName": "fileName"}]
                }
            }
        ]
    }
}

url = f"{SEARCH_ENDPOINT}/indexes/{INDEX_NAME}?api-version={API_VERSION}"
data = json.dumps(index_definition).encode("utf-8")

req = urllib.request.Request(
    url,
    data=data,
    method="PUT",
    headers={
        "Content-Type": "application/json",
        "api-key": SEARCH_API_KEY
    }
)

try:
    with urllib.request.urlopen(req) as response:
        print(f"Success: {response.status}")
        print(json.dumps(json.loads(response.read()), indent=2))
except urllib.error.HTTPError as e:
    print(f"Error: {e.code}")
    print(e.read().decode())
