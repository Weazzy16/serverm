const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let cefInit = false;
let cefInitData = [];

let last_pausemenustatus = false;
let last_cursorstate = false;
let last_warningactive = false;
//mp.gui.chat.show(false);
let main_browser = null;
//main_browser.reload(true);
//const main_browser = browsers.new("package://interface/index.html");
//main_browser.markAsChat();


//package://interface/local.html

const getInterfaceUrl = (serverId) => {
	return 'package://interface/local.html';
}

gm.events.add('client.init.cef', async (serverId) => {
	mp.gui.cursor.visible = true;
	mp.game.ui.setPauseMenuActive(false);

	mp.gui.chat.show(false);

	if (main_browser !== null)
		main_browser.destroy();

	main_browser = browsers.new(getInterfaceUrl (serverId));
	main_browser.markAsChat();
});

mp.gui.emmit = (execute, log = 0) => {
	try
	{
		if (log) {
			const text = `[cef debug] ${execute}`;
			if (cefInit) main_browser.execute(`console.log(${JSON.stringify(text)})`);
			else cefInitData.push(`console.log(${JSON.stringify(text)})`);
			//if (cefInit) 
			//	mp.gui.chat.push(text);
		}
		if (cefInit) main_browser.execute (execute);
		else cefInitData.push (execute);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "utils/cef", "mp.gui.emmit", e.toString());
	}
}

mp.gui.json = (name, json) => {
	try
	{
		if (cefInit) 
			main_browser.execute (`${name}('${JSON.stringify (json)}')`);
		else 
			cefInitData.push (execute);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "utils/cef", "mp.gui.emmit", e.toString());
	}
}


//mp.gui.execute (`window.location='package://interface/index.html'`, false);
global.cefRendered = false;
gm.events.add("client:AuthInit", () => {
	cefInit = true;
	global.cefRendered = true;

	if (cefInitData.length) {
		cefInitData.forEach((execute) => {
			//mp.gui.execute (execute);
			main_browser.execute (execute);
		});
		cefInitData = [];
	}
});

gm.events.add("client:OnBrowserInit", () => {
	try
	{
		// Исправляем баг с курсором в меню паузы
		if (last_pausemenustatus == false && mp.game.ui.isPauseMenuActive())
		{
			last_pausemenustatus = true;
			last_cursorstate = mp.gui.cursor.visible;
			mp.gui.cursor.visible = false;
		}
		else if(last_pausemenustatus == true && !mp.game.ui.isPauseMenuActive())
		{
			last_pausemenustatus = false;
			mp.gui.cursor.visible = last_cursorstate;
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "utils/cef", "client:OnBrowserInit", e.toString());
	}
    
});

global.IsJsonString = (str) => {
    try 
	{
        JSON.parse(str);
    } 
	catch (e) 
	{
        return false;
    }
    return true;
}
