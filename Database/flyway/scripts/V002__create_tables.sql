-- Connect to the database
\connect persona_db;

CREATE TABLE
   IF NOT EXISTS FoodInventory (
      id INTEGER PRIMARY KEY,
      food_date_bought VARCHAR(10),
      food_stored_in_electronic BOOLEAN,
      food_health INTEGER,
      eaten BOOLEAN
   );

CREATE TABLE
   IF NOT EXISTS Businesses (
      id INTEGER PRIMARY KEY,
      business_name VARCHAR(50),
      business_type VARCHAR(50)
   );

CREATE TABLE
   IF NOT EXISTS StockInventory (
      id INTEGER PRIMARY KEY,
      business_id INTEGER REFERENCES Businesses (id),
      num_stocks INTEGER,
      date_bought VARCHAR(10)
   );

CREATE TABLE
   IF NOT EXISTS HomeOwningStatus (
      id INTEGER PRIMARY KEY,
      status_description VARCHAR(20)
   );

CREATE TABLE
   IF NOT EXISTS Personas (
      id BIGINT PRIMARY KEY,
      next_of_kin_id BIGINT REFERENCES Personas (id),
      partner_id BIGINT REFERENCES Personas (id),
      parent_id BIGINT REFERENCES Personas (id),
      birth_format_time VARCHAR(10),
      hunger INTEGER,
      health INTEGER,
      alive BOOLEAN,
      sick BOOLEAN,
      num_electronics_owned INTEGER,
      home_owning_status_id INTEGER REFERENCES HomeOwningStatus (id),
      food_inventory_id INTEGER REFERENCES FoodInventory (id),
      stock_inventory_id INTEGER REFERENCES StockInventory (id)
   );

CREATE TABLE
   IF NOT EXISTS EventTypes (
      id INTEGER PRIMARY KEY,
      event_name VARCHAR(10) -- 1 = marriages, 2 = children, 3 = adults, 4 = deaths
   );

CREATE TABLE
   IF NOT EXISTS EventOccurred (
      id BIGINT PRIMARY KEY,
      event_id INTEGER REFERENCES EventTypes (id),
      persona_id_1 BIGINT REFERENCES Personas (id),
      persona_id_2 BIGINT REFERENCES Personas (id),
      date_occurred VARCHAR(10)
   );
