resource "aws_iam_role" "beanstalk_ec2" {
  assume_role_policy    = "{\"Statement\":[{\"Action\":\"sts:AssumeRole\",\"Effect\":\"Allow\",\"Principal\":{\"Service\":\"ec2.amazonaws.com\"}}],\"Version\":\"2012-10-17\"}"
  description           = "Allows EC2 instances to call AWS services on your behalf."
  force_detach_policies = false
  managed_policy_arns   = ["arn:aws:iam::aws:policy/AWSElasticBeanstalkMulticontainerDocker", "arn:aws:iam::aws:policy/AWSElasticBeanstalkWebTier", "arn:aws:iam::aws:policy/AWSElasticBeanstalkWorkerTier"]
  max_session_duration  = 3600
  name                  = "aws-elasticbeanstalk-ec2"
  path                  = "/"
}

resource "aws_iam_instance_profile" "beanstalk_ec2" {
  name = "aws-elasticbeanstalk-ec2-profile"
  role = aws_iam_role.beanstalk_ec2.name
}

resource "aws_s3_bucket" "backend_beanstalk" {
  bucket        = "179530787873-deploy-bucket"
  force_destroy = true
}

resource "aws_elastic_beanstalk_application" "backend_beanstalk" {
  name        = "api-app"
  description = "App for API"
}

resource "aws_elastic_beanstalk_environment" "backend_beanstalk" {
  name                = "api-env"
  application         = aws_elastic_beanstalk_application.backend_beanstalk.name
  solution_stack_name = "64bit Amazon Linux 2023 v3.1.2 running .NET 8"
  tier                = "WebServer"
  cname_prefix        = "api-persona"

  lifecycle {
    ignore_changes = all
  }

  setting {
    namespace = "aws:ec2:vpc"
    name      = "VPCId"
    value     = module.vpc.vpc_id
    resource  = ""
  }
  setting {
    namespace = "aws:autoscaling:launchconfiguration"
    name      = "IamInstanceProfile"
    value     = aws_iam_instance_profile.beanstalk_ec2.name
    resource  = ""
  }
  setting {
    namespace = "aws:ec2:vpc"
    name      = "Subnets"
    value     = join(",", module.vpc.public_subnets)
    resource  = ""
  }
  setting {
    namespace = "aws:ec2:instances"
    name      = "InstanceTypes"
    value     = "t3.micro"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:healthreporting:system"
    name      = "SystemType"
    value     = "basic"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:application"
    name      = "Application Healthcheck URL"
    value     = "/"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:command"
    name      = "Timeout"
    value     = "60"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:command"
    name      = "IgnoreHealthCheck"
    value     = "true"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:managedactions"
    name      = "ManagedActionsEnabled"
    value     = "false"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "SERVER_PORT"
    value     = "5000"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:environment"
    name      = "EnvironmentType"
    value     = "LoadBalanced"
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:environment"
    name      = "LoadBalancerType"
    value     = "application"
    resource  = ""
  }
  setting {
    namespace = "aws:elbv2:listener:80"
    name      = "ListenerEnabled"
    value     = "false"
    resource  = ""
  }
  setting {
    namespace = "aws:elbv2:listener:80"
    name      = "Protocol"
    value     = "HTTP"
    resource  = ""
  }
  setting {
    namespace = "aws:elbv2:listener:443"
    name      = "ListenerEnabled"
    value     = "true"
    resource  = ""
  }
  setting {
    namespace = "aws:elbv2:listener:443"
    name      = "Protocol"
    value     = "HTTPS"
    resource  = ""
  }
  setting {
    namespace = "aws:elbv2:listener:443"
    name      = "SSLCertificateArns"
    value     = "arn:aws:acm:eu-west-1:179530787873:certificate/b5dd305b-2460-42e1-8357-0bbf21c9656c"
    resource  = ""
  }
  setting {
    namespace = "aws:ec2:vpc"
    name      = "AssociatePublicIpAddress"
    value     = "true"
    resource  = ""
  }
  setting {
    namespace = "aws:elb:healthcheck"
    name      = "Interval"
    value     = 60
    resource  = ""
  }
  setting {
    namespace = "aws:elb:healthcheck"
    name      = "Timeout"
    value     = 20
    resource  = ""
  }
  setting {
    namespace = "aws:autoscaling:asg"
    name      = "MinSize"
    value     = 1
    resource  = ""
  }
  setting {
    namespace = "aws:autoscaling:asg"
    name      = "MaxSize"
    value     = 1
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DB_USERNAME"
    value     = module.rds.db_instance_username
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DB_PASSWORD"
    value     = var.db_password
    resource  = ""
  }
  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DB_URL"
    value     = module.rds.db_instance_address
    resource  = ""
  }
}
