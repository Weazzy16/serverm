export default [
    { name: "Чип-тюнинг", type: "chip", handlings: [
        "fDriveBiasFront",
        "fBrakeBiasFront",
        "fHandBrakeForce",
        "fTractionCurveMax",
        "fTractionCurveMin",
        "fLowSpeedTractionLossMult",
        "fTractionCurveLateral"
    ], controllers: ["StabilizationSwitching"] },
    { name: "Колеса и развал", type: "wheels", handlings: [
        "wheelRadius",
        "wheelWidth",
        "wheelCamber",
        "wheelTrackWidth",
        "wheelHeight",
        "fSteeringLock"
    ], controllers: [] },
];