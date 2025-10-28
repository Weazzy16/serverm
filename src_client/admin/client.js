const browsers = mp.browsers;

global.checker = {
  menuOpen: false,
}

mp.events.add("start:AlcoSystem::client", () => {
 if (!global.checker.menuOpen) {
  AlcoSystem = browsers.new("http://package/cef/AlcoSystem/index.html");
  global.checker.menuOpen = true;
} else {
         global.checker.menuOpen = false;
	 AlcoSystem.destroy();
    }
});

mp.events.add("stop:AlcoSystem::client", () => {
  global.checker.menuOpen = false;
  AlcoSystem.destroy();
});

mp.events.add("update:AlcoSystem::client", (time) => {
  AlcoSystem.execute(`app.time='${time}'`);
});


