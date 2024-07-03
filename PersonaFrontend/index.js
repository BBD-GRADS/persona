let spawnPersonaButton = document.getElementById("spawnPersonabtn");
let marryPersonasButton = document.getElementById("marryPersonasBtn");
let checkResidentsButton = document.getElementById("checkResidentsBtn");
let buyFoodButton = document.getElementById("buyFoodBtn");
let buyElectronicButton = document.getElementById("buyElectronicBtn");
let buyStockButton = document.getElementById("buyStockBtn");
let sellStockButton = document.getElementById("sellStockBtn");

let personaSearchButton = document.getElementById("pSearchButton");
let searchInput = document.getElementById("pSearch");
let sellStockInput = document.getElementById("sellStockInput");

let personaIDTitle = document.getElementById("personaIDTitle");

let testAddEventButton = document.getElementById("testAddEvent");

spawnPersonaButton.addEventListener("click",spawnPersona);
checkResidentsButton.addEventListener("click",checkResidents);
buyFoodButton.addEventListener("click",buyFood);
buyElectronicButton.addEventListener("click",buyElectronic);
buyStockButton.addEventListener("click",buyStock);
sellStockButton.addEventListener("click",sellStock);
personaSearchButton.addEventListener("click",searchForPersona);

testAddEventButton.addEventListener("click",addEventBox);
let currEventDetails = "worm";
let currEventTime = "00:00";

let currentPersona = -1;
personaIDTitle.innerText = "Currently viewing Persona ID: " + currentPersona;

function searchForPersona()
{
    if(isEmpty(searchInput.value))
        {
            console.log("empty");
            alert("input a search value");
        }
        else
        {
            console.log("has a value");
            console.log("searching for persona " + searchInput.value);
        }

}

function spawnPersona()
{
    console.log("spawn persona");
}

function marryPersonas()
{
    console.log("marry these personas");
}

function checkResidents()
{
    console.log("check persona house residents");
    alert("4 residents in persona " + currentPersona + "'s house");
}

function buyFood()
{
    console.log("buy food for persona");
}

function buyElectronic()
{
    console.log("buy electronic for persona");
}

function buyStock()
{
    console.log("buy stock for persona");
}

function sellStock()
{
    if(isEmpty(sellStockInput.value))
    {
        console.log("no stock input");
    }
    else
    {
        console.log("sell stock value: " + sellStockInput.value);
    }
    
}

function isEmpty(input) {
    return !input.trim().length;
}

function addEventBox()
{
    const listNode = document.createElement("li");
    const detailsTextNode = document.createTextNode(currEventDetails);
    const timeTextNode = document.createTextNode(currEventTime);

    const h4Node = document.createElement("h4");
    const pNode = document.createElement("p");

    h4Node.appendChild(detailsTextNode);
    pNode.appendChild(timeTextNode);

    listNode.appendChild(h4Node);
    listNode.appendChild(pNode);
    let eventList = document.getElementById("eventList");
    eventList.appendChild(listNode);
}
