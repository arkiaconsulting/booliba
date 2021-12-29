resource "akcauth_authorization_code_client" "booliba" {
  client_id = "booliba"
  client_name = "Booliba"
  allowed_scopes = [ "openid", "profile", "booliba.basic" ]
  redirect_uris = ["io.identitymodel.native://callback"]
}