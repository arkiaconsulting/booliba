resource "akcauth_authorization_code_client" "booliba" {
  client_id = "booliba"
  client_name = "Booliba"
  allowed_scopes = []
  redirect_uris = ["https://localhost:9999/callback"]
}