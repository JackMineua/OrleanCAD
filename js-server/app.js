const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 8008 });


/*const WebSocket = require("ws").Server;
const HttpsServer = require('https').createServer;
const fs = require("fs");

var server_websocket = HttpsServer({
    cert: fs.readFileSync("./cert.crt"),
    key: fs.readFileSync("./key.key")
})
var wss = new WebSocket({
    server: server_websocket,
	perMessageDeflate: false
});

server_websocket.listen(8008, function() {
	console.log("Вебсокет запущен на порту 8008");
	console.log(wss);
});

server_websocket.on('error', function error(err) {
    console.error('WebSocket server error:', err);
});*/

const mysql = require('mysql2');

const connection = mysql.createConnection({
  host: '',
  user: '',
  password: '',
  database: ''
});

const CURRENT_CAD_VERSION = "VJH12093WJDFGBN2983Y4RJGBR";


/*

clients[clientId].Auth (bool)
clients[clientId].SteamId (string)
clients[clientId].Rank (string)
clients[clientId].Status (string 10-8, 10-7, 10-6)
clients[clientId].Deps (string 1:2:3:4)
clients[clientId].IpNow (string (ip when connect))
clients[clientId].Ip (string (ip in base))
clients[clientId].currentDep (string LEO, CIV, Disp, FD)
clients[clientId].sessionRankAndId (string "SGT 00/A-000)
clients[clientId].sessionName (string "Name Surname")
clients[clientId].civsCount (int)
clients[clientId].wepsCount (int)
clients[clientId].vehsCount (int)

*/


let CallData = [
  { active: "false", type: "911", shortdesc: "Стрельба", longdesc: "У бабушки инфаркт.", num: "1", address1: "Forum Drive 8090", address2: "Forum Drive 8092", units: "", time: "19:45:20" },
  { active: "false", type: "ДТП", shortdesc: "ДТП", longdesc: "Пьяный мужчина сбил кота и он валяется присмерти.", num: "2", address1: "Forum Drive 8090", address2: "Forum Drive 8092", units: "", time: "19:40:20" },
  { active: "false", type: "Dispatch", shortdesc: "Алкаши", longdesc: "Пьяница с ножом.", num: "3", address1: "Forum Drive 8090", address2: "Forum Drive 8092", units: "", time: "19:35:20" },
  { active: "false", type: "911", shortdesc: "Да", longdesc: "Клоун с топором.", num: "4", address1: "Forum Drive 8090", address2: "Forum Drive 8092", units: "", time: "19:30:20" },
  { active: "false", type: "Dispatch", shortdesc: "Мгм", longdesc: "Ограбление 24/7", num: "5", address1: "Forum Drive 8090", address2: "Forum Drive 8092", units: "", time: "19:25:20" },
  { active: "false", type: "", shortdesc: "", longdesc: "", num: "6", address1: "", address2: "", units: "", time: "" },
  { active: "false", type: "", shortdesc: "", longdesc: "", num: "7", address1: "", address2: "", units: "", time: "" },
  { active: "false", type: "", shortdesc: "", longdesc: "", num: "8", address1: "", address2: "", units: "", time: "" }
]; 

const whitelist_ips_set = new Set([
	"161.129.70.159", 
	"..."
]);

let playerLocation = {};

connection.connect(function(err){
    if (err) {
      return console.error("Ошибка: " + err.message);
    }
    else{
      console.log("Подключение к серверу MySQL успешно установлено");
    }
 });

const http = require('http');
const url = require('url');
const qs = require('querystring');

// Создаем HTTP сервер
const server = http.createServer((req, res) => {
	  const { pathname } = new URL(req.url, `http://${req.headers.host}`);
      const urlParts = req.url.split('/');
	  
	  if (urlParts[1] !== 'A123SDJFSAJ1239KJSDF1SDNFFLJ1') return; 
	  if (urlParts[2] === 'signal100') 
	  {
		res.writeHead(200, {'Content-Type': 'text/plain'});
		res.end(""+is100Active);
	  } 
	  
	  else if (urlParts[2] === "playerLocation")
	  {
		const playerId = urlParts[3].toString();
		const playerAddress = decodeURIComponent(urlParts[4].toString());
		const playerPostal = urlParts[5].toString();
		if (!playerLocation[playerId]) {
			playerLocation[playerId] = {};  // Создаем объект, если он не существует
		}
		playerLocation[playerId].Address = playerAddress;
		playerLocation[playerId].Postal = playerPostal;
		console.log(playerLocation[playerId]);
		res.writeHead(200, {'Content-Type': 'text/plain'});
		res.end("Player Location Sent");
	  }
	  
	  else if (urlParts[2] === 'policeCount') 
	  {
		res.writeHead(200, {'Content-Type': 'text/plain'});
		
		var resp = "";
		if(activeUnitsCount == 0) resp = "ZERO";
		if(activeUnitsCount > 0 && activeUnitsCount <= 3) resp = "LOW";
		if(activeUnitsCount > 3 && activeUnitsCount <= 7) resp = "MEDIUM";
		if(activeUnitsCount > 7) resp = "HIGH";
		res.end(""+resp);
	  }
	  
	  else
	  {
		res.writeHead(404, {'Content-Type': 'text/plain'});
		res.end('Not Found');
		console.log('HTTP 404 Sent');
	  }
	  console.log('HTTP Sent');
});

// Запускаем сервер на порте 3000
server.listen(3000, '85.159.230.224', () => {
  console.log('Сервер запущен на http://85.159.230.224:3000/');
});

// Список клиентов
let clients = {};

const TYPE_LOGIN = "LOGIN";
const TYPE_DEBUG = "DEBUG";
const TYPE_TIME = "TIME";
const TYPE_PANIC = "PANIC";
const TYPE_SIGNAL100 = "SIGNAL100";
const TYPE_CALLDATA = "CALLDATA";
const TYPE_ACTIVE_DISPATCH = "ACTIVE_DISPATCH";
const TYPE_NCIC_REQUEST = "NCIC_REQUEST";
const TYPE_NCIC_RESULT = "NCIC_RESULT";
const TYPE_PLATES_REQUEST = "PLATES_REQUEST";
const TYPE_PLATES_RESULT = "PLATES_RESULT";
const TYPE_WSNS_REQUEST = "WSNS_REQUEST";
const TYPE_WSNS_RESULT = "WSNS_RESULT";
const TYPE_ACTIVE_UNITS = "ACTIVE_UNITS";
const TYPE_STATUS = "STATUS";
const TYPE_SEND_CHARACTERS = "LOAD_CIVS";
const TYPE_SEND_WEPS = "LOAD_WEPS";
const TYPE_SEND_VEHS = "LOAD_VEHS";

let isPanicActive = 0;
let is100Active = 0;
let isDispatchActive = 0;
let excludeClientPanic = "";
let excludeClient100 = "";
let ActiveDispatchID = "Inactive";
let ActiveDispatchClient = "";

setInterval(()=> broadcast(TYPE_TIME, getCurrentTime().toString()), 1000);
setInterval(()=> sendPacket(TYPE_SIGNAL100, "", is100Active), 2000);
setInterval(()=> sendPacket(TYPE_PANIC, "", isPanicActive), 2000);
setInterval(()=> sendPacket(TYPE_ACTIVE_DISPATCH, "", isDispatchActive), 2000);
setInterval(()=> updateStatusString(), 2000);
setInterval(()=> sendCallData(), 2000);

let TotalCalls = 0;
let activeUnits = "";
let activeUnitsCount = 0;

function updateStatusString() {
    activeUnits = ''; // Инициализация activeUnits перед началом цикла

    for (let clientId in clients) {
        if (clients[clientId].Status === "10-8") {
            if (activeUnits.includes(clients[clientId].sessionRankAndId)) continue;
            else 
			{ 
				activeUnits += clients[clientId].sessionRankAndId + ",  "; 
				activeUnitsCount++;
			} // Добавляем идентификатор в строку
        }
        if (clients[clientId].Status == "10-6" || clients[clientId].Status == "10-7") {
            if (activeUnits.includes(clients[clientId].sessionRankAndId)) {
                const regex = new RegExp(clients[clientId].sessionRankAndId + '\\s*', 'g'); // Create a regex to match clientId followed by optional whitespace
                activeUnits = activeUnits.replace(regex, '');
				activeUnitsCount--;
            } else continue;
        }
    }

    activeUnits = activeUnits.trim().replace(/,\s*$/, "");
    sendPacket(TYPE_ACTIVE_UNITS, "", "");
}

function sendPacket(type, msg, stateFirst) {
	let formatted = "";
	let state = "";
	if(stateFirst == 1) state = "true";
	if(stateFirst == 0) state = "false";
	if(stateFirst == 2) state = "10-6";
	
	if(type == TYPE_PANIC)
	{
		broadcast(TYPE_PANIC, state+":"+excludeClientPanic);
	}
	if(type == TYPE_SIGNAL100)
	{
		broadcast(TYPE_SIGNAL100, state+":"+excludeClient100);
	}
	if(type == TYPE_ACTIVE_DISPATCH)
	{
		broadcast(TYPE_ACTIVE_DISPATCH, state+":"+ActiveDispatchID);
	}
	if(type == TYPE_ACTIVE_UNITS)
	{
		broadcast(TYPE_ACTIVE_UNITS, activeUnits);
	}
	if(type == TYPE_CALLDATA)
	{
		broadcast_no_dots(TYPE_CALLDATA, msg);
	}
}

function WSNSSearch(serial, clientId) {
	console.log(clients[clientId].currentDep);
	//if(clients[clientId].currentDep != "LEO" || clients[clientId].currentDep != "Disp") return sendToClient(clientId, TYPE_WSNS_RESULT, "false");
    const req1args = [serial];
    const req1 = "SELECT * FROM `weapons` WHERE `serial` = ?";

    connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            sendToClient(clientId, TYPE_WSNS_RESULT, "false");
        } else {
            const firstRow = results[0];
            const seriall = firstRow.serial;
            const ownerr = firstRow.owner;
            const modell = firstRow.model;
            const stateofcreatee = firstRow.stateofcreate;
            const dateofcreatee = firstRow.dateofcreate;
			
			sendToClient(clientId, TYPE_WSNS_RESULT, "true:" + seriall + ":" + ownerr + ":" + modell + ":" + dateofcreatee + ":" + stateofcreatee);
		}
	});
}

function PlatesSearch(plates, clientId) {
    const req1args = [plates];
    const req1 = "SELECT * FROM `plates` WHERE `plate` = ?";
	console.log(clients[clientId].currentDep);
	//if(clients[clientId].currentDep != "LEO" || clients[clientId].currentDep != "Disp") return sendToClient(clientId, TYPE_PLATES_RESULT, "false");

    connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            sendToClient(clientId, TYPE_PLATES_RESULT, "false");
        } else {
            const firstRow = results[0];
            const platee = firstRow.plate;
            const ownerr = firstRow.owner;
            const colorr = firstRow.color;
            const modell = firstRow.model;
            const stateofcreatee = firstRow.stateofcreate;
            const dateofcreatee = firstRow.dateofcreate;
            const isInBoloo = firstRow.isInBolo;
			const BoloDesc = firstRow.bolo_desc;
			const BoloCreate = firstRow.bolocreatedate;
			
			sendToClient(clientId, TYPE_PLATES_RESULT, "true:" + platee + ":" + ownerr + ":" + colorr + ":" + modell + ":" + stateofcreatee + ":" + dateofcreatee + ":" + isInBoloo + ":" + BoloDesc + ":" + BoloCreate);
		}
	});
}


function NCICSearch(name, dd, mm, yyyy, clientId) {
    const req1args = [name, dd, mm, yyyy];
    const req1 = "SELECT * FROM `civs` WHERE `name` = ? AND `dd` = ? AND `mm` = ? AND `yyyy` = ?";

    connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            sendToClient(clientId, TYPE_NCIC_RESULT, "false");
        } else {
            const firstRow = results[0];
            const civsName = firstRow.name;
            const medicine = firstRow.medicine;
            const vehLicType = firstRow.veh_lic_type;
            const wepLic = firstRow.wep_lic;
            const work = firstRow.work;
            const sex = firstRow.sex;
            let orders = 0;

            const req2args = [name, dd, mm, yyyy];
            const req2 = "SELECT * FROM `orders` WHERE `name` = ? AND `dd` = ? AND `mm` = ? AND `yyyy` = ?";

            connection.query(req2, req2args, (error2, results2, fields2) => {
				let type;
				let desc;
				let dateofwrite;
                if (error2) {
                    console.error('Ошибка выполнения запроса:', error2);
                    return;
                }

                if (results2.length === 0) {
                    orders = 0;
                } else {
                    orders = 1;
                    const secondRow = results2[0];
                    type = secondRow.type;
                    desc = secondRow.description;
                    dateofwrite = secondRow.dateofwrite;
                }
				const state = "true";
				
				if(clients[clientId].currentDep == "FD")
				{
					const responseData = orders == 0 ?
						`${state}:${civsName}:${dd}:${mm}:${yyyy}:${medicine}:::${work}:${orders}:${sex}` :
						`${state}:${civsName}:${dd}:${mm}:${yyyy}:${medicine}:::${work}:${orders}:${sex}:${type}:${desc}:${dateofwrite}`;

					sendToClient(clientId, TYPE_NCIC_RESULT, responseData);
				}
				else
				{
					const responseData = orders == 0 ?
						`${state}:${civsName}:${dd}:${mm}:${yyyy}:${medicine}:${vehLicType}:${wepLic}:${work}:${orders}:${sex}` :
						`${state}:${civsName}:${dd}:${mm}:${yyyy}:${medicine}:${vehLicType}:${wepLic}:${work}:${orders}:${sex}:${type}:${desc}:${dateofwrite}`;

					sendToClient(clientId, TYPE_NCIC_RESULT, responseData);
				}
            });
        }
    });
}

function CheckLogin(clientId, login, pass1)
{
	if(!whitelist_ips_set.has(clients[clientId].IpNow))
	{
		//return sendToClient(clientId, TYPE_LOGIN, "blacklisted");
		//return ws.terminate();
	}
	const req1args = [login];
    const req1 = "SELECT * FROM `accounts` WHERE `login` = ?";

    connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            sendToClient(clientId, TYPE_DEBUG, "Аккаунт не найден. ");
        } else {
            const firstRow = results[0];
            const passs = firstRow.pass;
            const ipp = firstRow.ip;
			const depss = firstRow.deps;
			const rankk = firstRow.rank;
			const steamIdd = firstRow.steamId;
			if(pass1 == passs) { sendToClient(clientId, TYPE_LOGIN, "true;" + depss + ";" + rankk); clients[clientId].Auth = true; clients[clientId].Deps = depss; clients[clientId].Ip = ipp; clients[clientId].Rank = rankk; clients[clientId].SteamId = steamIdd; clients[clientId].currentDep = ""; }
			if(pass1 != passs) sendToClient(clientId, TYPE_LOGIN, "false");
		}
	});
}

function getLastCallIDFromBase()
{
	connection.query('SELECT MAX(id) FROM calls', (error, results, fields) => {
		if(error) {
			console.error('Ошибка выбора последнего ID вызова:', error);
			return;
		}
		if(results.length === 0) {
			//TotalCalls = 0;
			console.log('Последний ID вызова: 0');
		}
		else
		{
			const lastId = results[0]['MAX(id)'];
			if(lastId == null)
			{
				TotalCalls = 0;
			}
			else TotalCalls = lastId;
			console.log('Последний ID вызова: ', TotalCalls);
		}
	});
}
getLastCallIDFromBase();

function AddCall(typee, shortdescc, longdescc, adress11, adress22) {
    TotalCalls++;
    for (let i = 7; i > 0; i--) {
        CallData[i] = CallData[i - 1];
    }
    CallData[0] = {
        active: "true",
        type: typee,
        shortdesc: shortdescc,
        longdesc: longdescc,
        address1: adress11,
        address2: adress22,
		units: "",
        num: TotalCalls.toString(),
		time: getCurrentTime()
    };
	
	const dataQuery = {
		id: CallData[0].num,
		type: CallData[0].type,
		shortdesc: CallData[0].shortdesc,
		longdesc: CallData[0].longdesc,
		address1: CallData[0].address1,
		address2: CallData[0].address2,
		time: CallData[0].time
	};
	
	connection.query('INSERT INTO calls SET ?', dataQuery, (error, results, fields) => {
    if (error) {
      console.error('Ошибка выполнения запроса:', error);
      return;
    }
    console.log('Данные успешно внесены в таблицу calls');
  });
}

function DeleteCall(id) {
    if (id < 0 || id >= CallData.length) {
        return; // Не допускаем некорректных индексов
    }
    for (let i = id; i < CallData.length - 1; i++) {
        CallData[i] = CallData[i + 1];
    }
    CallData.pop(); // Удаляем последний элемент
}


function CreateCiv(clientId, params)
{
	const param = params.toString().split(':');
	const dataQuery = {
		owner_id: clientId,
		name: param[1],
		dd: param[2],
		mm: param[3],
		yyyy: param[4],
		medicine: param[5],
		veh_lic_type: param[6],
		wep_lic: param[7],
		work: param[8],
		sex: param[9]
	};
	connection.query('INSERT INTO civs SET ?', dataQuery, (error, results, fields) => {
		if (error) {
		  console.error('Ошибка выполнения запроса:', error);
		  return;
		}
	});
    console.log('Данные успешно внесены в таблицу civs');
	LoadCivs(clientId);
}

function CreateWep(clientId, params)
{
	const param = params.toString().split(':');
	const dataQuery = {
		owner_id: clientId,
		serial: param[1],
		model: param[2],
		owner: param[3],
		dateofcreate: param[4],
		stateofcreate: param[5]
	};
	connection.query('INSERT INTO weapons SET ?', dataQuery, (error, results, fields) => {
		if (error) {
		  console.error('Ошибка выполнения запроса:', error);
		  return;
		}
	});
    console.log('Данные успешно внесены в таблицу weapons!');
	LoadWeps(clientId);
}

function CreateVeh(clientId, params)
{
	const param = params.toString().split(':');
	const dataQuery = {
		owner_id: clientId,
		plate: param[1],
		owner: param[2],
		color: param[3],
		model: param[4],
		stateofcreate: param[5],
		dateofcreate: param[6]
	};
	connection.query('INSERT INTO plates SET ?', dataQuery, (error, results, fields) => {
		if (error) {
		  console.error('Ошибка выполнения запроса:', error);
		  return;
		}
	});
    console.log('Данные успешно внесены в таблицу plates!');
	LoadVehs(clientId);
}


function LoadCivs(clientId)
{
	if(clients[clientId].currentDep != "CIV") return;
	const req1args = [clientId];
    const req1 = "SELECT * FROM `civs` WHERE `owner_id` = ?";
	
	connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            return;
			console.log("У " + clientId + " аккаунта не найдено персонажей.");
        } else {
			sendToClientWithS(clientId, TYPE_SEND_CHARACTERS, JSON.stringify(results));
			clients[clientId].civsCount = results.length;
		}
	});
}

function LoadVehs(clientId)
{
	if(clients[clientId].currentDep != "CIV") return;
	const req1args = [clientId];
    const req1 = "SELECT * FROM `plates` WHERE `owner_id` = ?";
	
	connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            return;
			console.log("У " + clientId + " аккаунта не найдено транспортов.");
        } else {
			sendToClientWithS(clientId, TYPE_SEND_VEHS, JSON.stringify(results));
			clients[clientId].vehsCount = results.length;
		}
	});
}

function LoadWeps(clientId)
{
	if(clients[clientId].currentDep != "CIV") return;
	const req1args = [clientId];
    const req1 = "SELECT * FROM `weapons` WHERE `owner_id` = ?";
	
	connection.query(req1, req1args, (error, results, fields) => {
        if (error) {
            console.error('Ошибка выполнения запроса:', error);
            return;
        }
        if (results.length === 0) {
            return;
			console.log("У " + clientId + " аккаунта не найдено оружий.");
        } else {
			sendToClientWithS(clientId, TYPE_SEND_WEPS, JSON.stringify(results));
			clients[clientId].wepsCount = results.length;
		}
	});
}

/*

clients[clientId].Auth (bool)
clients[clientId].SteamId (string)
clients[clientId].Rank (string)
clients[clientId].Status (string 10-8, 10-7, 10-6)
clients[clientId].Deps (string 1:2:3:4)
clients[clientId].IpNow (string (ip when connect))
clients[clientId].Ip (string (ip in base))
clients[clientId].currentDep (string LEO, CIV, Disp, FD)
clients[clientId].sessionRankAndId (string "SGT 00/A-000)
clients[clientId].sessionName (string "Name Surname")

*/

wss.on('connection', function connection(ws, req) {
  console.log(req.url);
  const urlParams = req.url.split('/'); // Разбиваем URL по слешу
  const clientId = urlParams[1];
  
  clients[clientId] = ws;
  
  const clientIp = req.connection.remoteAddress;
  const cleanedIp = clientIp.slice(clientIp.lastIndexOf(':') + 1);
  clients[clientId].IpNow = cleanedIp;
  
  
  console.log('Client connected: ' + clientId + ". IP: " + clients[clientId].IpNow);

  ws.on('message', function incoming(message) {
	const parts_of_message = message.toString().split(':');
	const requestType = parts_of_message[0];
	
	/*clients[clientId] = {
		currentDep: "",
		Auth: false,
		Rank: "Cadet",
		Status: "INACTIVE",
		Deps: "",
		IpNow: "",
		Ip: "",
		sessionRankAndId: "",
		sessionName: ""
	};*/
	
    if (requestType === 'LOGIN') {
	  const parts = message.toString().split(':');
	  var login = parts[1];
	  var pass = parts[2];
	  var version = parts[3];
	  if(version != CURRENT_CAD_VERSION) return ws.terminate();
	  CheckLogin(clientId, login, pass);
    }
	
	else 
	{
		if(clients[clientId].Auth != true) return ws.terminate();
		if (requestType === 'PANIC') {
		  const parts = message.toString().split(':');
		  var state = parts[1];
		  if(state == "true") { 
			isPanicActive = 1; 
			excludeClientPanic = clientId; 
			if(!playerLocation[clients[clientId].SteamId]) { playerLocation[clients[clientId].SteamId].Address = "Неизвестно"; playerLocation[clients[clientId].SteamId].Postal = "Неизвестно"; }
			//if(!playerLocation[clients[clientId].SteamId].Postal) playerLocation[clients[clientId].SteamId].Postal = "Неизвестно";
			AddCall("Panic", "Паника!", "Офицер " + clients[clientId].sessionName + " " + clients[clientId].sessionRankAndId + " активировал кнопку паники! Ему нужна помощь!", playerLocation[clients[clientId].SteamId].Address + " " + playerLocation[clients[clientId].SteamId].Postal, playerLocation[clients[clientId].SteamId].Address + " " + playerLocation[clients[clientId].SteamId].Postal);
			CallData[0].units = CallData[0].units + "" + clients[clientId].sessionRankAndId;
		  }
		  if(state == "false") { 
			isPanicActive = 0; 
			excludeClientPanic = clientId; 
			const index = CallData.findIndex(item => item.type === "Panic");
			DeleteCall(index);
		  }
		  broadcast_except(clientId, TYPE_PANIC, state+":"+clientId);
		}
		
		if(requestType == "DELETE_CALL")
		{
			const parts = message.toString().split(':');
			if(clients[clientId].currentDep != "Disp") return;
			DeleteCall(parseInt(parts[1]));
		}
		
		if(requestType == "UNIT_SESSION_DATA")
		{
			const parts = message.toString().split(':');
			var data = parts[1];
			const data_parts = data.toString().split(' ');
			var data1 = data_parts[2];
			var data2 = data_parts[3];
			var name = data_parts[0];
			var surname = data_parts[1];
			if (data2 == null) data2 = "";
			var fullname = name + " " + surname;
			var finaldata = data1 + " " + data2;
			clients[clientId].sessionRankAndId = finaldata;
			clients[clientId].sessionName = fullname;
		}
		
		if(requestType == "CREATE_CALL")
		{
			if(!clients[clientId].Deps.includes("4") || !clients[clientId].Deps.includes("1")) return ws.terminate();
			
			const parts = message.toString().split(':');
			
			console.log(parts[4]);
			
			if(clients[clientId].currentDep == "CIV") AddCall("911", parts[3], parts[4], parts[1], parts[2]);
			if(clients[clientId].currentDep == "Disp") AddCall(parts[5], parts[3], parts[4], parts[1], parts[2]);
		}
		
		if(requestType == "CREATE_CIV")
		{
			if(!clients[clientId].Deps.includes("4")) return ws.terminate();
			if(clients[clientId].civsCount >= 4) return;
			
			CreateCiv(clientId, message);
		}
		
		if(requestType == "CREATE_WEP")
		{
			if(!clients[clientId].Deps.includes("4")) return ws.terminate();
			if(clients[clientId].wepsCount >= 4) return;
			
			CreateWep(clientId, message);
		}
		
		if(requestType == "CREATE_VEH")
		{
			if(!clients[clientId].Deps.includes("4")) return ws.terminate();
			if(clients[clientId].vehsCount >= 4) return;
			
			CreateVeh(clientId, message);
		}
		
		if(requestType == "CHANGE_DEP")
		{
		  const parts = message.toString().split(':');
		  var dep = parts[1];
		  if(!clients[clientId].Deps.includes(dep)) return ws.terminate();
		  if(dep == 1) clients[clientId].currentDep = "LEO";
		  if(dep == 2) clients[clientId].currentDep = "FD";
		  if(dep == 3) clients[clientId].currentDep = "Disp";
		  if(dep == 4) 
		  {
			  clients[clientId].currentDep = "CIV";
			  LoadCivs(clientId);
			  LoadWeps(clientId);
			  LoadVehs(clientId);
		  }
		  
		  playerLocation[clients[clientId].SteamId] = {
			Address: "Неизвестно",
			Postal: "0000",
		  };
		  
		  for(let i = 0; i < 8; i++)
		  {
			  if(!CallData[i]) continue;
			  if(CallData[i].units.includes(clients[clientId].sessionRankAndId))
			  {
				const updatedUnits = CallData[i].units.split(',  ').filter(unit => unit !== clients[clientId].sessionRankAndId);

				// Обновляем значение units в CallData
				CallData[i].units = updatedUnits.join(',  ');
			  }
			  else continue;
		  }
		  if(ActiveDispatchClient == clientId)
		  {
			isDispatchActive = 0;
			ActiveDispatchID = "Inactive";
			ActiveDispatchClient = "";
		  }
		}
		
		if (requestType === 'SIGNAL100') {
		  const parts = message.toString().split(':');
		  var state = parts[1];
		  if(state == "true") { is100Active = 1; excludeClient100 = clientId; }
		  if(state == "false") { is100Active = 0; excludeClient100 = clientId; }
		  broadcast_except(clientId, TYPE_SIGNAL100, state+":"+clientId);
		}
		
		if (requestType === 'CALL_UNIT_ADD') {
		  const parts = message.toString().split(':');
		  var unit = parts[1];
		  var call = parts[2];
		  CallData[parseInt(call)].units = CallData[parseInt(call)].units + ",  " + clients[unit].sessionRankAndId;
		  sendToClient(unit, TYPE_STATUS, "10-6");
		  clients[unit].Status = "10-6";
		}
		
		if (requestType === 'CALL_UNIT_REMOVE') {
			const parts = message.toString().split(':');
			const unitToRemove1 = parts[1];
			const unitToRemove = clients[unitToRemove1].sessionRankAndId;
			const call = parts[2];

			// Проверяем, есть ли юнит в указанном вызове
			if (CallData[call] && CallData[call].units) {
				// Разбиваем строку на массив по разделителю '  ' и удаляем указанный юнит
				const updatedUnits = CallData[call].units.split(',  ').filter(unit => unit !== unitToRemove);

				// Обновляем значение units в CallData
				CallData[call].units = updatedUnits.join(',  ');

				// Отправляем сообщение клиенту и обновляем статус
				sendToClient(unitToRemove1, TYPE_STATUS, "10-8");
				if (clients[unitToRemove1]) {
					clients[unitToRemove1].Status = "10-8";
				}
			} else {
				console.error('Указанный вызов или юнит не найдены.');
			}
		}
		
		if (requestType === 'NCIC_REQUEST') {
		  const parts = message.toString().split(':');
		  var name = parts[1];
		  var dd = parts[2];
		  var mm = parts[3];
		  var yyyy = parts[4];
		  
		  NCICSearch(name, dd, mm, yyyy, clientId);
		}
		
		if (requestType === 'PLATES_REQUEST') {
		  const parts = message.toString().split(':');
		  var plates = parts[1];
		  
		  PlatesSearch(plates, clientId);
		}
		
		if (requestType === 'WSNS_REQUEST') {
		  const parts = message.toString().split(':');
		  var plates = parts[1];
		  
		  WSNSSearch(plates, clientId);
		}
		
		if (requestType === 'STATUS') {
		  const parts = message.toString().split(':');
		  var statusnum = parts[1];
		  
		  if(statusnum == "ACTIVE") 
		  {
			  clients[clientId].Status = "10-8";
			  if(clients[clientId].currentDep == "Disp")
			  {
				  isDispatchActive = 1;
				  ActiveDispatchID = clients[clientId].sessionRankAndId;
				  ActiveDispatchClient = clientId;
			  }
		  }
		  if(statusnum == "BUSY") 
		  {
			  clients[clientId].Status = "10-6";
			  if(clients[clientId].currentDep == "Disp")
			  {
				  isDispatchActive = 2;
				  ActiveDispatchID = clients[clientId].sessionRankAndId;
				  ActiveDispatchClient = clientId;
			  }
		  }
		  if(statusnum == "INACTIVE") 
		  {
			  clients[clientId].Status = "10-7";
			  if(clients[clientId].currentDep == "Disp")
			  {
				  isDispatchActive = 0;
				  ActiveDispatchID = "Inactive";
				  ActiveDispatchClient = "";
			  }
		  }
		}
	}
  });

  ws.on('close', function() {
    console.log('Client disconnected:', clientId);
	if(excludeClient100 == clientId) { is100Active = 0; excludeClient100 = ""; }
	if(excludeClientPanic == clientId) { isPanicActive = 0; excludeClientPanic = ""; }
    if(ActiveDispatchClient == clientId) { isDispatchActive = 0; ActiveDispatchID = "Inactive"; ActiveDispatchClient = ""; }
    delete clients[clientId];
  });
});

function sendCallData()
{
	Object.values(clients).forEach(function(client) {
		if(client.currentDep != "") 
		{
			if(client.currentDep == "LEO" || client.currentDep == "Disp")
			{
				sendPacket(TYPE_CALLDATA, JSON.stringify(CallData), 0);
			}
		}
	});
}

function getCurrentTime()
{
	let date = new Date();
	let hours = date.getHours();
	let minutes = date.getMinutes();
	let seconds = date.getSeconds();
	
	let currentTime = "" + hours + ":" + minutes + ":" + seconds;
	
	return currentTime;
}

function broadcast(type, some_msg) {
  Object.values(clients).forEach(function(client) {
    if (client.readyState === WebSocket.OPEN) {
      client.send(type+":"+some_msg);
    }
  });
}

function broadcast_no_dots(type, some_msg) {
  Object.values(clients).forEach(function(client) {
    if (client.readyState === WebSocket.OPEN) {
      client.send(type+"&"+some_msg);
    }
  });
}

function broadcast_except(excludedClient, type, some_msg) {
	Object.values(clients).forEach(function(client) {
	  if (client !== excludedClient) {
		if (client.readyState === WebSocket.OPEN) {
		  client.send(type+":"+some_msg);
		}
	  }
  });
}

// Функция для отправки сообщения конкретному клиенту
function sendToClient(clientId, type, some_msg) {
  const client = clients[clientId];
  if (client && client.readyState === WebSocket.OPEN) {
    client.send(type+":"+some_msg); // Используем переменную some_msg
  }
}

function sendToClientWithS(clientId, type, some_msg) {
  const client = clients[clientId];
  if (client && client.readyState === WebSocket.OPEN) {
    client.send(type+"&"+some_msg); // Используем переменную some_msg
  }
}
