#resource "aws_secretsmanager_secret" "auth_secret_key" {
#  name = "auth_secret_key"
#}

#resource "aws_secretsmanager_secret_version" "auth_secret_key" {
#  secret_id     = aws_secretsmanager_secret.auth_secret_key.id
#  secret_string = "dummy"
#}

#data "aws_secretsmanager_secret_version" "auth_secret_key" {
#  secret_id  = aws_secretsmanager_secret.auth_secret_key.arn
#  depends_on = [aws_secretsmanager_secret_version.auth_secret_key]
#}





#
#resource "aws_cognito_user_pool" "prd_pool" {
#  name                     = var.db_name
#  schema {
#    name                = "email"
#    attribute_data_type = "String"
#    mutable             = true
#    required            = true
#  }
#  auto_verified_attributes = ["email"]
#}

resource "aws_cognito_user_pool" "prd_pool_1" {
  name                     = var.domain_name
  schema {
    name                = "email"
    attribute_data_type = "String"
    mutable             = true
    required            = true
  }
  auto_verified_attributes = ["email"]
}

resource "aws_cognito_user_pool_client" "prd_pool" {
  name                         = "prd_client"
  user_pool_id                 = aws_cognito_user_pool.prd_pool_1.id
  access_token_validity        = 1
  supported_identity_providers = ["Google"]
  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows          = ["implicit", "code"]
  allowed_oauth_scopes         = ["email", "openid", "profile", "aws.cognito.signin.user.admin"]
  callback_urls                = ["https://persona.projects.bbdgrad.com", "http://localhost:4200"]
  generate_secret              = true
  
  depends_on = [aws_cognito_identity_provider.google]
}

resource "aws_cognito_user_pool_domain" "prd_pool" {
  domain       = "persona-manager"
  user_pool_id = aws_cognito_user_pool.prd_pool_1.id
}


resource "aws_cognito_identity_provider" "google" {
  user_pool_id  = aws_cognito_user_pool.prd_pool_1.id
  provider_name = "Google"
  provider_type = "Google"

  provider_details = {
    authorize_scopes              = "email openid profile"
    client_id                     = var.google_client_id
    client_secret                 = var.client_secret
    token_url                     = "https://www.googleapis.com/oauth2/v4/token"
    token_request_method          = "POST"
    oidc_issuer                   = "https://accounts.google.com"
    authorize_url                 = "https://accounts.google.com/o/oauth2/v2/auth"
    attributes_url                = "https://people.googleapis.com/v1/people/me?personFields="
    attributes_url_add_attributes = "true"
  }

  attribute_mapping = {
    email          = "email"
    username       = "sub"
    email_verified = "email_verified"
  }
}