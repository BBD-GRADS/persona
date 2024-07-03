let currentPersona = -1;

let addChildButton = document.getElementById("spawnChildbtn");
let marryPersonasButton = document.getElementById("marryPersonasBtn");
let checkResidentsButton = document.getElementById("checkResidentsBtn");
let buyFoodButton = document.getElementById("buyFoodBtn");
let buyElectronicButton = document.getElementById("buyElectronicBtn");
let buyStockButton = document.getElementById("buyStockBtn");
let sellStockButton = document.getElementById("sellStockBtn");
let checkStockButton = document.getElementById("checkStockBtn");


let personaSearchButton = document.getElementById("pSearchButton");
let searchInput = document.getElementById("pSearch");
let sellStockInput = document.getElementById("sellStockInput");

let personaIDTitle = document.getElementById("personaIDTitle");

let testAddEventButton = document.getElementById("testAddEvent");

let nextOfKin = document.getElementById("nextOfKin");
let partner = document.getElementById("partner");
let parent = document.getElementById("parent");
let birthTime = document.getElementById("birthTime");
let hunger = document.getElementById("hunger");
let health = document.getElementById("health");
let alive = document.getElementById("alive");
let sick = document.getElementById("sick");
let electronics = document.getElementById("electronics");
let homeOwner = document.getElementById("homeOwnerStatus");
let foodInventory = document.getElementById("foodInventory");
let stockInventory = document.getElementById("stockInventory");
let adult = document.getElementById("adult");


addChildButton.addEventListener("click",spawnChild);
checkResidentsButton.addEventListener("click",checkResidents);
buyFoodButton.addEventListener("click",buyFood);
buyElectronicButton.addEventListener("click",buyElectronic);
buyStockButton.addEventListener("click",buyStock);
sellStockButton.addEventListener("click",sellStock);
personaSearchButton.addEventListener("click",searchForPersona);
checkStockButton.addEventListener("click",checkStock);


personaIDTitle.innerText = "Currently viewing Persona ID: " + currentPersona;

function searchForPersona()
{
    if(isEmpty(searchInput.value))
        {
            alert("input a search value");
        }
        else
        {
            updatePersonaDetails(searchInput.value,personaTestObj);
            
        }

}

function spawnChild()
{
    populateResponseBlock("Spawned a child for persona " + currentPersona);
}


function checkResidents()
{
    populateResponseBlock("There are 4 residents in " + currentPersona + "'s house");
}

function buyFood()
{
    populateResponseBlock("Bought food for persona " + currentPersona );
}

function buyElectronic()
{
    populateResponseBlock("Bought electronic for persona " + currentPersona);
}

function buyStock()
{
    populateResponseBlock("Bought stock for persona " + currentPersona);
}

function sellStock()
{
    if(isEmpty(sellStockInput.value))
    {
        alert("Please input a stock value");
    }
    else
    {
        populateResponseBlock("Sold stock with id: " + sellStockInput.value);
    }
    
}

function checkStock()
{
    populateResponseBlock("Persona " + currentPersona + " has the following stocks: 89, 76, 23");
}

function populateResponseBlock(content)
{
    document.getElementById("responseText").innerText = content;
}

function updatePersonaDetails(personaID, personaJSON)
{
    currentPersona = personaID;
    personaIDTitle.innerText = "Currently viewing Persona ID: " + currentPersona;
    console.log(personaJSON);

    nextOfKin.innerText = personaJSON.next_of_kin_id;
    partner.innerText = personaJSON.partner_id;
    parent.innerText = personaJSON.parent_id;
    birthTime.innerText = personaJSON.birth_date;
    hunger.innerText = personaJSON.hunger;
    health.innerText = personaJSON.health;
    alive.innerText = personaJSON.alive;
    sick.innerText = personaJSON.sick;
    electronics.innerText = personaJSON.num_electronics_owned;
    homeOwner.innerText = personaJSON.home_owning_status;
    foodInventory.innerText = personaJSON.num_food_items;
    stockInventory.innerText = personaJSON.num_stocks_owned;
    adult.innerText = personaJSON.adult;


}



function isEmpty(input) {
    return !input.trim().length;
}

personaTestObj = JSON.parse('{"id": 2,"parent_id": 1,"partner_id": 3,"next_of_kin_id": 5,"birth_date": "01|10|19","hunger": 88,"health": 100,"alive": true,"sick": false,"num_electronics_owned": 3,"home_owning_status": "owner","num_food_items": 5,"num_stocks_owned": 3,"adult": true}')
