variable "account_id" {
  type        = string
  description = "AWS account ID"
}

variable "db_name" {
  type        = string
  description = "Name for the DB"
}

variable "db_username" {
  type        = string
  description = "DB username"
}

variable "db_password" {
  type        = string
  description = "DB password"
}

variable "domain_name" {
  type        = string
  description = "Base domain name for backend and frontend"
}

variable "google_client_id" {
  type        = string
  description = "Google client ID for OAuth"
}

variable "client_secret" {
  type        = string
  description = "Google client secret for OAuth"
}

variable "access_key" {
  type        = string
  description = "Access key (no looky!)"
}

variable "secret_key" {
  type        = string
  description = "Secret key (no looky!)"
}
