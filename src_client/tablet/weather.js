const
    clientName = "client.tablet.",
    rpcName = "rpc.tablet.",
    serverName = "server.tablet.";

tabletData.weatherList = [];

gm.events.add(clientName + "initWeather", (json) => {
    tabletData.weatherList = [];

    json = JSON.parse(json);

    let weatherList = [];

    json.forEach((item) => {
        weatherList.push({
            weatherId: item[0],
            hour: item[1],
            minute: item[2],
            temp: item[3]
        });
    });

    tabletData.weatherList = weatherList;
});

rpc.register(rpcName + "getWeather", () => {
    return JSON.stringify(tabletData.weatherList.slice(0, 6));
});

rpc.register(rpcName + "getCurrentWeather", () => {
    return JSON.stringify(tabletData.weatherList [0]);
});

gm.events.add(clientName + "addWeather", (weatherId, hour, minute, temp) => {
    tabletData.weatherList.splice(0, 1);

    tabletData.weatherList.push({
        weatherId: weatherId,
        hour: hour,
        minute: minute,
        temp: temp
    });
});

gm.events.add(clientName + "updWeather", (weatherId, temp) => {
    tabletData.weatherList [0].weatherId = weatherId;
    tabletData.weatherList [0].temp = temp;
});