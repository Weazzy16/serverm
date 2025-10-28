<script>
    import { onMount, createEventDispatcher } from "svelte";
    import { executeClient, executeClientAsync } from 'api/rage';
    import { addListernEvent } from "api/functions";
    import "./css/notifications.scss";

    const dispatch = createEventDispatcher();

    let notifications = [];

    function close() {
        dispatch("close");
    }

    addListernEvent("cef.capture.getNotifications", (data) => {
        notifications = JSON.parse(data);
    });

    onMount(() => {
        executeClient ("cl.capture.getNotifications")
    });
</script>

<div class="notification-container">
    <div class="container">
      <header>
        <h1>Список уведомлений</h1>
        <button on:click={close}>
          <svg
            width="8"
            height="8"
            viewBox="0 0 8 8"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M7.84185 0.966939C7.94448 0.860868 8.00126 0.718803 7.99998 0.571342C7.9987 0.42388 7.93945 0.282822 7.83499 0.178547C7.73054 0.0742723 7.58923 0.0151244 7.44152 0.0138431C7.2938 0.0125617 7.15149 0.0692491 7.04523 0.171696L4.00692 3.20476L0.968613 0.171696C0.916643 0.11798 0.854477 0.0751352 0.785742 0.04566C0.717008 0.0161849 0.643081 0.000670149 0.568276 2.12348e-05C0.493471 -0.000627679 0.419285 0.0136021 0.350047 0.0418804C0.28081 0.0701587 0.217907 0.111919 0.16501 0.164725C0.112113 0.217531 0.0702801 0.280325 0.0419529 0.349443C0.0136256 0.41856 -0.000628766 0.492618 2.12716e-05 0.567294C0.000671309 0.64197 0.0162129 0.715769 0.0457391 0.784385C0.0752653 0.853001 0.118185 0.915059 0.171993 0.966939L3.2103 4L0.171993 7.03306C0.118185 7.08494 0.0752653 7.147 0.0457391 7.21562C0.0162129 7.28423 0.000671309 7.35803 2.12716e-05 7.43271C-0.000628766 7.50738 0.0136256 7.58144 0.0419529 7.65056C0.0702801 7.71967 0.112113 7.78247 0.16501 7.83527C0.217907 7.88808 0.28081 7.92984 0.350047 7.95812C0.419285 7.9864 0.493471 8.00063 0.568276 7.99998C0.643081 7.99933 0.717008 7.98381 0.785742 7.95434C0.854477 7.92486 0.916643 7.88202 0.968613 7.8283L4.00692 4.79524L7.04523 7.8283C7.15149 7.93075 7.2938 7.98744 7.44152 7.98616C7.58923 7.98487 7.73054 7.92573 7.83499 7.82145C7.93945 7.71718 7.9987 7.57612 7.99998 7.42866C8.00126 7.2812 7.94448 7.13913 7.84185 7.03306L4.80354 4L7.84185 0.966939Z"
              
            />
          </svg>
        </button>
      </header>
  
      <!-- <div class="divider"></div> -->
  
      <div class="list-notifications">
        {#each notifications as notify}
          <div class="notification">
            <header>
              <div class="state">НОВОЕ</div>
              <div class="date">{notify.date}</div>
            </header>
            <div>{notify.text}</div>
            <div class="receiver">
              Кому отправлено:
              <div class="name">{notify.receiver}</div>
            </div>
          </div>
        {/each}
      </div>
    </div>
  </div>
  