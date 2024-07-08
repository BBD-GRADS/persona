import {ApiHelper } from "./apiHelper.js";
import {clientId, cognitoDomain } from './config.js';

let currentPersona = -1;
const baseURL = "https://api.persona.projects.bbdgrad.com/api";
const stockURL = "https://api.mese.projects.bbdgrad.com";
const foodURL = "https://api.sustenance.projects.bbdgrad.com/api";
const electronicURL = "https://service.electronics.projects.bbdgrad.com";

let addChildButton = document.getElementById("spawnChildbtn");
let buyFoodButton = document.getElementById("buyFoodBtn");
let buyElectronicButton = document.getElementById("buyElectronicBtn");
let buyStockButton = document.getElementById("buyStockBtn");
let refreshDataButton = document.getElementById("refreshData");
let logoutbutton = document.getElementById("logoutbtn");


let personaSearchButton = document.getElementById("pSearchButton");
let searchInput = document.getElementById("pSearch");
//let sellStockInput = document.getElementById("sellStockInput");

let personaIDTitle = document.getElementById("personaIDTitle");


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
buyFoodButton.addEventListener("click",buyFood);
buyElectronicButton.addEventListener("click",buyElectronic);
buyStockButton.addEventListener("click",buyStock);
//sellStockButton.addEventListener("click",sellStock);
personaSearchButton.addEventListener("click",searchForPersona);
refreshDataButton.addEventListener("click",populateDataBlocks);
logoutbutton.addEventListener("click",logout);

let aliveValue = document.getElementById("aliveValue");
let deceasedValue = document.getElementById("deceasedValue");
let birthsValue = document.getElementById("birthsValue");
let marriagesValue = document.getElementById("marriagesValue");


if (localStorage.getItem("accessToken") != null) {
    //we all good
    }else
    {
        console.log("no access token!!!");
        window.location.href = 'https://persona.projects.bbdgrad.com/login.html';
    }




populatePersonaID(currentPersona);
populateDataBlocks();

function logout()
{
    localStorage.removeItem("accessToken");
    window.location.href = 'https://persona.projects.bbdgrad.com/login.html';
}

function populatePersonaID(id)
{
    if(id == -1)
    {
        personaIDTitle.innerText = "No persona ID";
    }
    else
    {  
        personaIDTitle.innerText = "Currently viewing Persona ID: " + currentPersona;
    }
}


async function populateDataBlocks()
{
    let personaAliveData = ((await getAlivePersonas()).data);
    console.log(personaAliveData);
    aliveValue.innerText = personaAliveData.personas.length;

    let personaDeceasedData = ((await getDeadPersonas()).data)
    deceasedValue.innerText = personaDeceasedData.personas.length;

    let personaBirthsData = ((await getBirthedPersonas()).data)
    if((personaBirthsData - 1000) < 0)
        {
            birthsValue.innerText = "0";
        }else{
            birthsValue.innerText = (personaBirthsData - 1000);
        }

    let personaMarriagesData = ((await getMarriedPersonas()).data)
    marriagesValue.innerText = personaMarriagesData;

}

async function searchForPersona()
{
    if(isEmpty(searchInput.value))
        {
            alert("input a search value");
        }
        else
        {
        //updatePersonaDetails(searchInput.value,personaTestObj);
        console.log(await getPersona(searchInput.value));
        let personaObject = (await getPersona(searchInput.value));
        if(personaObject != undefined)
            {
                updatePersonaDetails(searchInput.value,personaObject.data);
            }
        }

}

async function spawnChild()
{
    if(currentPersona != -1 )
    {
        await makeChild(currentPersona);
        //populateResponseBlock("Spawned a child for persona " + currentPersona);
    }
    
}

function buyFood()
{
    buyFoods();
}

function buyElectronic()
{
    buyElectronics();
}

function buyStock()
{
    buyStocks();
}

// function sellStock()
// {
//     if(isEmpty(sellStockInput.value))
//     {
//         alert("Please input a stock value");
//     }
//     else
//     {
//         populateResponseBlock("Sold stock with id: " + sellStockInput.value);
//     }
    
// }

function checkStock()
{
    populateResponseBlock("Persona " + currentPersona + " has the following stocks: 89, 76, 23");
}

function populateResponseBlock(content)
{
    if(content)
    {
        document.getElementById("responseText").innerText = content;
    }
    else{
        document.getElementById("responseText").innerText = "Could not show response";
    }
}

function updatePersonaDetails(personaID, personaJSON)
{
    try
    {
    currentPersona = personaID;
    personaIDTitle.innerText = "Currently viewing Persona ID: " + currentPersona;
    console.log(personaJSON);

    nextOfKin.innerText = personaJSON.nextOfKinId;
    partner.innerText = personaJSON.partnerId;
    parent.innerText = personaJSON.parentId;
    birthTime.innerText = personaJSON.birthFormatTime;
    hunger.innerText = personaJSON.hunger;
    health.innerText = personaJSON.health;
    alive.innerText = personaJSON.alive;
    sick.innerText = personaJSON.sick;
    electronics.innerText = personaJSON.numElectronicsOwned;
    homeOwner.innerText = showHomeOwnerState(personaJSON.homeOwningStatusId);
    foodInventory.innerText = personaJSON.foodInventory.length;
    stockInventory.innerText = personaJSON.stockInventory.length;
    adult.innerText = personaJSON.adult;
    }catch{
        console.log("Could not display Persona data");
    }
}

function isEmpty(input) {
    return !input.trim().length;
}

function showHomeOwnerState(id)
{
    switch(id)
    {
        case 2:
            return "home owner";
        case 3:
            return "renter";
        default:
            return "homeless";
    }
}

//personaTestObj = JSON.parse('{"id": 2,"parent_id": 1,"partner_id": 3,"next_of_kin_id": 5,"birth_date": "01|10|19","hunger": 88,"health": 100,"alive": true,"sick": false,"num_electronics_owned": 3,"home_owning_status": "owner","num_food_items": 5,"num_stocks_owned": 3,"adult": true}')


async function getPersona(id) {
    const apiHelper = new ApiHelper(baseURL);
      let persona = "";
    
      try {
        const response = await apiHelper.get('/Persona/' + id);
          
        if (response) {
          persona = response;
        }
      } catch (error) {
        console.error('Error performing CRUD operation:', error);
      }
      
      return persona;
      
    }

async function getAlivePersonas() {
    const apiHelper = new ApiHelper(baseURL);
        let alivePersonas = [];
    
        try {
        const response = await apiHelper.get('/Persona/getAlivePersonas');
            
        if (response) {
            alivePersonas = response;
        }
        } catch (error) {
        console.error('Error performing CRUD operation:', error);
        }
        
        return alivePersonas;
        
    }

async function getDeadPersonas() {
    const apiHelper = new ApiHelper(baseURL);
        let deadPersonas = [];
    
        try {
        const response = await apiHelper.get('/Persona/DeadPersons');
            
        if (response) {
            deadPersonas = response;
        }
        } catch (error) {
        console.error('Error performing CRUD operation:', error);
        }
        
        return deadPersonas;  
    }

async function getBirthedPersonas() {
    const apiHelper = new ApiHelper(baseURL);
        let birthedPersonas = [];
    
        try {
        const response = await apiHelper.get('/Persona/Births');
            
        if (response) {
            birthedPersonas = response;
        }
        } catch (error) {
        console.error('Error performing CRUD operation:', error);
        }
        
        return birthedPersonas;
        
    }

async function getMarriedPersonas() {
    const apiHelper = new ApiHelper(baseURL);
        let marriedPersonas = [];
    
        try {
        const response = await apiHelper.get('/Persona/Married');
            
        if (response) {
            marriedPersonas = response;
        }
        } catch (error) {
        console.error('Error performing CRUD operation:', error);
        }
        
        return marriedPersonas;
    }

                

async function makeChild(id) {
    const apiHelper = new ApiHelper(baseURL);
    let body =
    {
        "parentId": id
    }
    try {
      const response = await apiHelper.post('Persona/makeNewChild',body);
      console.log(response);
      populateResponseBlock(response.message);
    } catch (error) {
      console.error('Error performing CRUD operation:', error);
    }
  }
  

  async function buyStocks() {
    const apiHelper = new ApiHelper(stockURL);
    let id = currentPersona;
    try {
      const response = await apiHelper.post('stocks/buy/' + id);
      console.log(response);
      populateResponseBlock(response.message);
    } catch (error) {
      console.error('Error performing CRUD operation:', error);
    }
  }

  async function buyFoods() {
    const apiHelper = new ApiHelper(foodURL);
    let id = currentPersona;
    try {
      const response = await apiHelper.get('/Buy?consumerId=' + id);
      populateResponseBlock(response.message);
      console.log(response);
    } catch (error) {
      console.error('Error performing CRUD operation:', error);
    }
  }

  async function buyElectronics() {
    const apiHelper = new ApiHelper(electronicURL);
    let body = {
        "personaId" : currentPersona.toString(),
        "quanity"   : 1
    }
    console.log(body);
    try {
      const response = await apiHelper.post('store/order',body);
      console.log(response);
      populateResponseBlock(response.message);
    } catch (error) {
      console.error('Error performing CRUD operation:', error);
    }
  }

  async function verifyAccessToken(accessToken) {
    const url = `https://${cognitoDomain}/oauth2/tokeninfo`;
    console.log(url);
  
    accessToken = localStorage.getItem("accessToken");

    const body = new URLSearchParams({
        access_token: accessToken,
      });
    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: body,
      });
  
      if (!response.ok) {
        throw new Error('Failed to verify access token');
      }
  
      const data = await response.json();
      console.log('Token verification response:', data);
  
      // Check the response to determine if the token is valid
      if (data.hasOwnProperty('error')) {
        console.error('Access token verification failed:', data.error);
        //window.location.href = 'https://persona.projects.bbdgrad.com/login.html';
        return false;
      } else {
        console.log('Access token is valid');
        // Optionally, extract and use information from 'data' if needed
        console.log(data.email);
        return true;
      }
    } catch (error) {
      console.error('Error verifying access token:', error);
      //window.location.href = 'https://persona.projects.bbdgrad.com/login.html';
      return false;
    }
  }
  

 
