CREATE TABLE "HomeOwningStatus" (
  "id" serial PRIMARY KEY,
  "status_description" varchar(20)
);
 
CREATE TABLE "Businesses" (
  "id" serial PRIMARY KEY,
  "business_name" varchar(50),
  "business_type" varchar(50)
);
 
CREATE TABLE "Personas" (
  "id" bigserial PRIMARY KEY,
  "next_of_kin_id" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "partner_id" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "parent_id" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "birth_format_time" varchar(10),
  "hunger" integer,
  "health" integer,
  "adult" boolean,
  "days_starving" integer,
  "alive" boolean,
  "sick" boolean,
  "num_electronics_owned" integer,
  "home_owning_status_id" integer REFERENCES "HomeOwningStatus" ("id") ON DELETE SET NULL
);
 
CREATE TABLE "StockItems" (
  "id" serial PRIMARY KEY,
  "persona_id" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "business_id" integer REFERENCES "Businesses" ("id") ON DELETE SET NULL,
  "num_stocks" integer,
  "date_bought" varchar(10)
);
 
CREATE TABLE "FoodItems" (
  "id" serial PRIMARY KEY,
  "persona_id" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "food_date_bought" varchar(10),
  "food_stored_in_electronic" boolean,
  "food_health" integer,
  "food_eaten" boolean
);
 
CREATE TABLE "EventTypes" (
  "id" serial PRIMARY KEY,
  "event_name" varchar(8)
);
 
CREATE TABLE "EventsOccurred" (
  "id" bigserial PRIMARY KEY,
  "event_id" int REFERENCES "EventTypes" ("id") ON DELETE SET NULL,
  "persona_id_1" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "persona_id_2" bigint REFERENCES "Personas" ("id") ON DELETE SET NULL,
  "date_occurred" varchar(10)
);