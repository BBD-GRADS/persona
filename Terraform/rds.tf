data "aws_availability_zones" "available_zones" {
  
}

resource "aws_default_subnet" "subnet_az1" {
  availability_zone = data.aws_availability_zones.available_zones.names[0]
}

resource "aws_default_subnet" "subnet_az2" {
  availability_zone = data.aws_availability_zones.available_zones.names[1]
}


resource "aws_security_group" "allow_mssql_current" {
  name        = "allow_mssql_current"

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "allow_mssql_current"
    owner = "keegan.oreilly@bbd.co.za"
    created-using = "terraform"
  }
}

resource "aws_security_group" "allow_postgres_current" {
  name        = "allow_postgres"
  description = "Allow PostgreSQL traffic"
  vpc_id      = module.vpc.vpc_id

  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "allow_postgres"
  }
}

resource "aws_security_group" "rds" {
  name_prefix = "rds-"

  vpc_id = module.vpc.vpc_id
  ingress {
    from_port   = "5432"
    to_port     = "5432"
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
}



module "rds" {
  source  = "terraform-aws-modules/rds/aws"
  version = "6.5.2"

  identifier = var.db_name

  family               = "postgres16"
  major_engine_version = "16"
  engine               = "postgres"
  engine_version       = "16"

  instance_class      = "db.t3.micro"
  create_db_instance  = true
  allocated_storage   = 10
  deletion_protection = false
  skip_final_snapshot = true

  db_subnet_group_name   = module.vpc.database_subnet_group_name
  vpc_security_group_ids = [aws_security_group.allow_postgres_current.id]
  publicly_accessible    = true

  db_name  = var.db_name
  username = var.db_username
  port     = "5432"

  tags = {
    owner         = "keegan.oreilly@bbd.co.za"
    created-using = "terraform"
  }
}