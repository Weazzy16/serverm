<script>
  import { onMount } from "svelte";
  import BeatCapture from "./BeatCapture.svelte";
  import InfoCapture from "./InfoCapture.svelte";
  import NotificationCapture from "./NotificationCapture.svelte";
  import "./css/main.scss";
  import atacksImage from "./assets/attacks.png";
  import defendsImage from "./assets/defends.png";
  import bellImage from "./assets/bell.svg";
  import { executeClient } from "api/rage";
  import { addListernEvent } from "api/functions";

  export let viewData;

  let show = false;
  let countSquares = 0;
  let squaresInfo = [];
  let myFractionName = "Marabunta Grande";

  let avaibleAttacks = 3;
  let avaibleDefends = 3;

  let leftSquares = [
    "empty",
    "empty",
    "empty",
    "square",
    "square",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
    // "empty",
    "square",
    "square",
    "square",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "square",
    "square",
    "square",
    "square",
  ];

  let rightSquares = [
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    // "empty",
    "square",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "empty",
    "empty",
    "empty",
    "square",
    "square",
    // "empty",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    "square",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    // "empty",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    "square",
    "square",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    // "empty",
    "square",
    "square",
    "empty",
    "empty",
    "empty",
    "square",
    "square",
  ];
  addListernEvent("cef.capture.load", (countSquares, squares, name) => {
    countSquares = countSquares;
    squaresInfo = JSON.parse(squares);
    myFractionName = name;
    refreshData();
  });
  onMount(() => {
    try {
      console.log("MOUNTED");
      console.log(viewData);
      if (viewData) {
        show = viewData[0];
        countSquares = viewData[1];
        squaresInfo = JSON.parse(viewData[2]);
        myFractionName = viewData[3];
        refreshData();
      }
    } catch (e) {
      console.log(e);
    }
  });
  const refreshData = () => {
    $: leftSquaresComputed = getSquares(leftSquares, 1);
    $: rightSquaresComputed = getSquares(rightSquares, 49);

    //посчитать кол-во нападений

    var myAttacks = squaresInfo.filter(
      (el) => el.inWar === true && el.attackGangName === myFractionName
    );
    var existsAttacks = 3 - myAttacks.length;
    if (existsAttacks < 0) existsAttacks = 0;
    avaibleAttacks = existsAttacks;

    //посчитать кол-во защит

    var myDefends = squaresInfo.filter(
      (el) => el.inWar === true && el.fractionName === myFractionName
    );
    var existsDefends = 3 - myDefends.length;
    if (existsDefends < 0) existsDefends = 0;
    avaibleDefends = existsDefends;
  };
  let popups = {
    infoCapture: {
      active: false,
      idSquare: 0,
      isOwner: false,
      attackGangName: "Marabunta",
      defendGangName: "Bloods",
      countMembers: 1,
      calibre: "-",
      startDate: "22 июля, 15:22",
    },
    beatCapture: {
      active: false,
      territoryUnderControl: "Marabunta",
      idSquare: 0,
      countMembers: 2,
      hour: 0,
      minute: 0,
      calibreIndex: 0,
      armor: false,
      resist: false,
      selectedMembers: [],
      nonSelectedMembers: [],
    },
    notifications: {
      active: true,
    },
  };

  const tryShowBeatCapture = (e, square) => {
    if (square.index < 1) return;
    if (square.inWar) {
      const squareInfo = squaresInfo.find((el) => el.index === square.index);
      const localX = e.clientX - e.target.offsetLeft;
      const localY = e.clientY - e.target.offsetTop;
      showInfoCapture(
        localX,
        localY,
        squareInfo.index,
        myFractionName === squareInfo.attackGangName,
        squareInfo.attackGangName,
        squareInfo.defendGangName,
        squareInfo.countMembers,
        squareInfo.calibreIndex,
        squareInfo.startDate
      );

      return;
    }

    showBeatCapture(square);
  };
  const eventShowBeatCapture = (event) => {
    const { square, extraData } = event.detail;
    showBeatCapture(square, extraData);
  };
  const showBeatCapture = (square, extraData = false) => {
    console.log(square);
    popups.beatCapture.idSquare = square.index;
    popups.beatCapture.territoryUnderControl = square.fractionName;

    if (extraData) {
      popups.beatCapture.hour = square.hour;
      popups.beatCapture.minute = square.minute;
      popups.beatCapture.countMembers = square.countMembers;
      popups.beatCapture.armor = square.armor;
      popups.beatCapture.resist = square.resist;
      popups.beatCapture.calibreIndex = square.calibreIndex;
      popups.beatCapture.selectedMembers = square.selectedMembers;
      popups.beatCapture.nonSelectedMembers = square.nonSelectedMembers;
    } else {
      popups.beatCapture.hour = 0;
      popups.beatCapture.minute = 0;
      popups.beatCapture.countMembers = 2;
      popups.beatCapture.armor = false;
      popups.beatCapture.resist = false;
      popups.beatCapture.calibreIndex = 0;
      popups.beatCapture.selectedMembers = [];
      popups.beatCapture.nonSelectedMembers = [];
    }

    popups.beatCapture.active = true;
  };

  const hideBeatCapture = () => {
    popups.beatCapture.active = false;
  };

  const showNotifications = () => {
    popups.notifications.active = true;
  };

  const hideNotifications = () => {
    popups.notifications.active = false;
  };

  const showInfoCapture = (
    x,
    y,
    idSquare,
    isOwner,
    attackGangName,
    defendGangName,
    countMembers,
    calibreIndex,
    startDate
  ) => {
    popups.infoCapture = {
      x: x,
      y: y,
      active: true,
      idSquare,
      isOwner,
      attackGangName,
      defendGangName,
      countMembers,
      calibreIndex,
      startDate,
    };
  };

  const hideInfoCapture = () => {
    popups.infoCapture.active = false;
  };

  const getSquares = (list, fromIndex) => {
    if (squaresInfo) {
      //   if (Array.isArray(squaresInfo) == false) {
      //     squaresInfo = JSON.parse(squaresInfo);
      //   }
      console.log(squaresInfo);
      console.log("correct json");
    } else {
      console.log(squaresInfo);

      console.log("INcorrect json!!");
    }

    return list.map((sq) => {
      try {
        const square = squaresInfo.find((el) => el?.index === fromIndex);
        const val = sq === "empty" ? "empty-square" : "square " + square?.color;
        if (val !== "empty-square") fromIndex++;
        const result = {
          key: Date.now(),
          value: val,
          inWar: square?.inWar,
          fractionName: square?.fractionName,
          index: val === "empty-square" ? -1 : square?.index,
        };
        return result;
      } catch (e) {
        console.log(
          e,
          "index:",
          fromIndex,
          "overall squareInfo length:",
          squaresInfo.length
        );
      }
    });
  };

  const close = () => {
    executeClient("client.capture.close");
    show = false;
  };

  $: getPercentsOfTerritory = ((countSquares * 100) / 70).toFixed(2);

  $: leftSquaresComputed = getSquares(leftSquares, 0);
  $: rightSquaresComputed = getSquares(rightSquares, 48);
</script>

<div class="capture_tablet_main">
  {#if show}
    <div class="popups">
      {#if popups.infoCapture.active}
        <InfoCapture
          class="fade-in"
          on:close={hideInfoCapture}
          on:open-beat-capture={eventShowBeatCapture}
          x={popups.infoCapture.x}
          y={popups.infoCapture.y}
          isOwner={popups.infoCapture.isOwner}
          idSquare={popups.infoCapture.idSquare}
          attackGangName={popups.infoCapture.attackGangName}
          defendGangName={popups.infoCapture.defendGangName}
          countMembers={popups.infoCapture.countMembers}
          calibreIndex={popups.infoCapture.calibreIndex}
          startDate={popups.infoCapture.startDate}
        />
      {/if}

      {#if popups.notifications.active}
        <NotificationCapture class="fade-in" on:close={hideNotifications} />
      {/if}
    </div>

    <div class="capture-tablet">
      {#if popups.beatCapture.active}
        <BeatCapture
          on:close={hideBeatCapture}
          territoryUnderControl={popups.beatCapture.territoryUnderControl}
          idSquare={popups.beatCapture.idSquare}
          hour={popups.beatCapture.hour}
          minute={popups.beatCapture.minute}
          countMember={popups.beatCapture.countMembers}
          armor={popups.beatCapture.armor}
          resist={popups.beatCapture.resist}
          calibreIndex={popups.beatCapture.calibreIndex}
          propSelectedMembers={popups.beatCapture.selectedMembers}
          propNonSelectedMembers={popups.beatCapture.nonSelectedMembers}
        />
      {:else}
        <section class="screen">
          <header class="header">
            <button on:click={close}>
              <!-- <img class="arrow-back" src="./assets/arrow-back.png" alt="" /> -->
              <svg
                width="9"
                height="14"
                viewBox="0 0 9 14"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M0.238905 7.49966L7.58961 13.7933C7.91216 14.0689 8.43473 14.0689 8.75809 13.7933C9.08064 13.5178 9.08064 13.0702 8.75809 12.7946L1.99043 7.00035L8.75728 1.20605C9.07982 0.930496 9.07982 0.482921 8.75728 0.206667C8.43473 -0.0688891 7.91134 -0.0688891 7.5888 0.206667L0.23809 6.50028C-0.0794991 6.77299 -0.079499 7.22759 0.238905 7.49966Z"
                />
              </svg>
            </button>

            <section class="control">
              <div class="control-info">
                <span>Контроль:</span>
                {getPercentsOfTerritory}%<span> | </span>
                {countSquares}<span> / 70</span>
              </div>
            </section>

            <section class="attacks">
              <img src={atacksImage} alt="" />{avaibleAttacks} / 3
            </section>
            <section class="defends">
              <img src={defendsImage} alt="" />{avaibleDefends} / 3
            </section>
            <button class="notifications" on:click={showNotifications}
              ><img src={bellImage} alt="" /></button
            >
          </header>

          <section class="zones">
            <div class="grid-squares-left">
              {#each leftSquaresComputed as square}
                <div
                  class="square_wrapper"
                  on:click={(e) => tryShowBeatCapture(e, square)}
                >
                  <div class={square.value}>
                    {#if square.inWar}
                      <div class="skull" />
                    {/if}
                  </div>
                  <div class="text">
                    {#if square.index !== -1 && !square.inWar}
                      {square.index}
                    {/if}
                  </div>
                </div>
              {/each}
            </div>

            <div class="grid-squares-right">
              {#each rightSquaresComputed as square}
                <div
                  class="square_wrapper"
                  on:click={(e) => tryShowBeatCapture(e, square)}
                >
                  <div class={square.value}>
                    {#if square.inWar}
                      <div class="skull" />
                    {/if}
                  </div>
                  <div class="text">
                    {#if square.index !== -1 && !square.inWar}
                      {square.index}
                    {/if}
                  </div>
                </div>
              {/each}
            </div>
          </section>
        </section>
      {/if}
    </div>
  {/if}
</div>
