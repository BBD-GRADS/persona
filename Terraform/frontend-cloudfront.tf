#resource "aws_acm_certificate" "frontend" {
#  domain_name       = "web.${var.domain_name}"
#  validation_method = "DNS"

#  lifecycle {
#    create_before_destroy = true
#  }

#  provider = aws.us-east-1
#}

resource "aws_cloudfront_origin_access_control" "frontend" {
  name                              = "Frontend"
  description                       = "Allow cloudfront access to bucket with frontend html"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "frontend" {
  origin {
    domain_name              = aws_s3_bucket.frontend.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.frontend.id
    origin_id                = "primaryS3"
  }

  enabled             = true
  is_ipv6_enabled     = false
  default_root_object = "index.html"

  # aliases = [ var.domain_name ]

  default_cache_behavior {
    # Using the CachingDisabled managed policy ID:
    cache_policy_id        = "4135ea2d-6df8-44a3-9df3-4b5a84be39ad"
    allowed_methods        = ["GET", "HEAD", "OPTIONS"]
    cached_methods         = ["GET", "HEAD"]
    target_origin_id       = "primaryS3"
    viewer_protocol_policy = "https-only"
  }

  price_class = "PriceClass_200"

  restrictions {
    geo_restriction {
      restriction_type = "none"
      locations        = []
    }
  }

  viewer_certificate {
    acm_certificate_arn      = "arn:aws:acm:us-east-1:179530787873:certificate/f13f601d-b7a9-4023-918d-cd1a099188bf"
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }

  custom_error_response {
    error_caching_min_ttl = 10
    error_code            = "403"
    response_code         = "200"
    response_page_path    = "/index.html"
  }
}