<script>
  import { createEventDispatcher, onMount } from "svelte";
  import { addListernEvent } from "api/functions";
  import { executeClient, executeClientAsync } from "api/rage";
  import "./css/beat.scss";
  import bellImage from "./assets/bell.svg";
  import doubleArrowImage from "./assets/double-arrow.svg";

  export let territoryUnderControl;
  export let idSquare;
  export let countMembers = 2;
  export let hour = 0;
  export let minute = 0;
  export let calibreIndex = 0;
  export let armor = false;
  export let resist = false;
  export let propSelectedMembers = [];
  export let propNonSelectedMembers = [];

  const dispatch = createEventDispatcher();

  let rules = {
    hour,
    minute,
    countMembers,
    calibreIndex,
    armor,
    resist,
  };

  let calibres = [
    "PistolAmmo",
    "RiflesAmmo",
    "ShotgunsAmmo",
    "SMGAmmo",
    "SniperAmmo",
  ];

  $: selectedMembers = [];
  $: nonSelectedMembers = [];
  $: searchName = "";
  $: searchNameSelected = "";

  // $: members = { selectedMembers, nonSelectedMembers, searchName, searchNameSelected };
  // let members = {
  //   selectedMembers,
  //   nonSelectedMembers,
  //   searchName: "",
  //   searchNameSelected: "",
  // };

  function close() {
    dispatch("close");
  }

  function beatCapture() {
    // if (selectedMembers.length < rules.countMembers)
    //     return;

    executeClient(
      "client.toserver.capture.beatCapture",
      idSquare,
      rules.countMembers,
      rules.hour,
      rules.minute,
      calibres[calibreIndex],
      JSON.stringify(selectedMembers.map((x) => x.uuid)),
      rules.armor,
      rules.resist,
      rules.calibreIndex
    );

    close();
  }

  function incrementHour() {
    if (rules.hour < 23) {
      rules.hour += 1;
    }
  }

  function decrementHour() {
    if (rules.hour > 0) {
      rules.hour -= 1;
    }
  }

  function incrementMinute() {
    if (rules.minute < 59) {
      rules.minute += 1;
    } else {
      rules.minute = 0;
    }
  }

  function decrementMinute() {
    if (rules.minute > 0) {
      rules.minute -= 1;
    } else {
      rules.minute = 59;
    }
  }

  function incrementCountMembers() {
    rules.countMembers += 1;
  }

  function decrementCountMembers() {
    if (rules.countMembers > 2 && selectedMembers.length < rules.countMembers) {
      rules.countMembers -= 1;
    }
  }

  function leftSwitchCalibre() {
    rules.calibreIndex =
      rules.calibreIndex > 0 ? rules.calibreIndex - 1 : calibres.length - 1;
  }

  function rightSwitchCalibre() {
    rules.calibreIndex =
      rules.calibreIndex < calibres.length - 1 ? rules.calibreIndex + 1 : 0;
  }

  // function selectMember(member) {
  //   if (selectedMembers.length < rules.countMembers) {
  //     const index = nonSelectedMembers.indexOf(member);

  //     if (index != -1) {
  //       selectedMembers.push(member);
  //       nonSelectedMembers.splice(index, 1);
  //     }
  //   }
  // }

  // function unSelectMember(member) {
  //   const index = selectedMembers.indexOf(member);

  //   if (index != -1) {
  //     nonSelectedMembers.push(member);
  //     selectedMembers.splice(index, 1);
  //   }
  // }
  const selectMember = (member) => {
    if (selectedMembers.length < rules.countMembers) {
      console.log(member);
      const index = nonSelectedMembers.findIndex(
        (el) => el.uuid === member.uuid
      );
      console.log(index);

      if (index != -1) {
        selectedMembers = [...selectedMembers, member]; // обновление реактивной переменной
        nonSelectedMembers = [
          ...nonSelectedMembers.slice(0, index),
          ...nonSelectedMembers.slice(index + 1),
        ]; // обновление реактивной переменной
        console.log(nonSelectedMembers, selectedMembers);
      }
    }
  };

  const unSelectMember = (member) => {
    console.log(member);
    const index = selectedMembers.findIndex((el) => el.uuid === member.uuid);
    console.log(index);

    if (index != -1) {
      nonSelectedMembers = [...nonSelectedMembers, member]; // обновление реактивной переменной
      selectedMembers = [
        ...selectedMembers.slice(0, index),
        ...selectedMembers.slice(index + 1),
      ]; // обновление реактивной переменной
      console.log(nonSelectedMembers, selectedMembers);
    }
  };

  addListernEvent("cef.capture.getMembers", (data) => {
    // console.log(data);
    nonSelectedMembers = JSON.parse(data);
    nonSelectedMembers = nonSelectedMembers.filter(
      (el) => !selectedMembers.find((x) => x.uuid === el.uuid)
    );
  });

  onMount(() => {
    rules.hour = hour == undefined ? 0 : hour;
    rules.minute = minute == undefined ? 0 : minute;
    rules.countMembers = countMembers == undefined ? 3 : countMembers;
    rules.calibreIndex = calibreIndex == undefined ? 0 : calibreIndex;
    rules.armor = armor == undefined ? false : armor;
    rules.resist = resist == undefined ? false : resist;

    nonSelectedMembers =
      propNonSelectedMembers == undefined ? [] : propNonSelectedMembers;
    selectedMembers =
      propSelectedMembers == undefined ? [] : propSelectedMembers;

    if (nonSelectedMembers.length == 0) {
      executeClient("client.toserver.capture.getMembers", idSquare);
    }
  });
</script>

<div class="beat_capture-container">
  <header class="header">
    <button on:click={close}>
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
    <div class="square-id">{idSquare}</div>
    <div class="control">
      Контроль: <span>{territoryUnderControl}</span>
    </div>
    <button class="beat-capture-btn" on:click={beatCapture}
      >Запланировать атаку</button
    >
  </header>

  <div class="divider"></div>

  <section class="rules">
    <div class="name">
      <div>Время начала атаки</div>
      <div style="margin-left: 230px;">Участники каждой из сторон</div>
      <div style="margin-left: 15px;">Калибр оружия</div>
      <div style="margin-left: 162px;">Резист</div>
      <div style="margin-left: 10px;">Бронежилет</div>
    </div>
    <div class="settings">
      <div class="increment-input">
        <div>Часы</div>
        <button on:click={decrementHour}>-</button>
        <input type="text" readonly bind:value={rules.hour} />
        <button on:click={incrementHour} class="plus">+</button>
      </div>
      <div class="increment-input">
        <div>Минуты</div>
        <button on:click={decrementMinute}>-</button>
        <input type="text" readonly bind:value={rules.minute} />
        <button on:click={incrementMinute} class="plus">+</button>
      </div>
      <div class="increment-input count-members">
        <div>Участники</div>
        <button on:click={decrementCountMembers}>-</button>
        <input type="text" readonly bind:value={rules.countMembers} />
        <button on:click={incrementCountMembers} class="plus">+</button>
      </div>
      <div class="switch-input">
        <div>Калибр</div>
        <button on:click={leftSwitchCalibre}>&lt;</button>
        <input type="text" readonly bind:value={calibres[rules.calibreIndex]} />
        <button on:click={rightSwitchCalibre} class="plus">&gt;</button>
      </div>
      <div class="checkbox-input">
        <label class="container">
          <input type="checkbox" bind:checked={rules.resist} />
          <span class="checkmark"></span>
        </label>
      </div>
      <div class="checkbox-input armor">
        <label class="container">
          <input type="checkbox" bind:checked={rules.armor} />
          <span class="checkmark"></span>
        </label>
      </div>
    </div>
  </section>

  <div class="divider"></div>

  <section class="members">
    <section class="all-members">
      <div class="labels">
        <div class="label">Участники организации</div>
        <div class="label" style="margin-left: auto; margin-right: 22px;">
          Всего: {nonSelectedMembers.length + selectedMembers.length}
        </div>
      </div>

      <input
        type="text"
        class="search-input"
        placeholder="Поиск по списку"
        bind:value={searchName}
      />

      <div class="info">
        <div class="field" style="margin-left: 10px">#</div>
        <div class="field" style="margin-left: 20px">Имя и Фамилия</div>
        <div class="field" style="margin-left: 98px">UUID</div>
        <div class="field" style="margin-left: auto; margin-right: 10px">
          Статус
        </div>
      </div>

      <div class="list-members">
        {#each searchName === "" ? nonSelectedMembers : nonSelectedMembers.filter( (x) => x.nickname
                  .toLowerCase()
                  .includes(searchName.toLowerCase()) ) as member, index}
          <div class="member" on:click={() => selectMember(member)}>
            <div
              class="field"
              style="margin-left: 10px; width: 30px; height: 30px"
            >
              <div class="arrows"><img src={doubleArrowImage} alt="" /></div>
              {index}
            </div>
            <div class="field" style="width: 195px">
              {member.nickname}
            </div>
            <div class="field" style="width: 98px">
              {member.uuid}
            </div>
            <div class="field" style="margin-left: auto; margin-right: 10px">
              {member.status}
            </div>
          </div>
        {/each}
      </div>
    </section>

    <section class="selected-members">
      <div class="labels">
        <div class="label" style="margin-left: auto; margin-right: 22px;">
          Всего: {selectedMembers.length}/{rules.countMembers}
        </div>
      </div>

      <input
        type="text"
        class="search-input"
        placeholder="Поиск по списку"
        bind:value={searchNameSelected}
      />

      <div class="info">
        <div class="field" style="margin-left: 10px">#</div>
        <div class="field" style="margin-left: 20px">Имя и Фамилия</div>
        <div class="field" style="margin-left: 98px">UUID</div>
        <div class="field" style="margin-left: auto; margin-right: 10px">
          Статус
        </div>
      </div>

      <div class="list-members">
        {#each searchNameSelected === "" ? selectedMembers : selectedMembers.filter( (x) => x.nickname.includes(searchNameSelected) ) as member, index}
          <div class="member selected" on:click={() => unSelectMember(member)}>
            <!-- <h1>
                <img
                  src={doubleArrowImage}
                  style="transform: rotateZ(180deg);"
                  alt=""
                />
              </h1> -->
            <div class="field" style="margin-left: 10px; width: 30px;">
              <div class="arrows">
                <img
                  src={doubleArrowImage}
                  alt=""
                  style="transform: rotateZ(180deg);"
                />
              </div>
              {index}
            </div>
            <div class="field" style="width: 195px">
              {member.nickname}
            </div>
            <div class="field" style="width: 98px">
              {member.uuid}
            </div>
            <div class="field" style="margin-left: auto; margin-right: 10px">
              {member.status}
            </div>
          </div>
        {/each}
      </div>
    </section>
  </section>
</div>
