import { writable } from 'svelte/store';
import { format } from 'api/formatter'

export const text = writable("");
export const selected = writable(false);

const _reportsDataStore = () => {

    let state = []
    const { subscribe, set } = writable(state);

    window.reportsStore = {
        addReport: (id_, author_, text_) => {
            text_ = format("parseDell", text_);
            state.unshift({
                id: id_,
                author: author_,
                text: text_,
                blocked: false,
                blockedBy: '',
                status: false,
    closed: false
            });
            set(state);
        },
         setStatus: (id, blockedBy, status, closed) => {
  const idx = state.findIndex(r => r.id == id);
  if (idx === -1) return;

  // игрок закрыл — просто помечаем, но не удаляем:
  state[idx].status   = Boolean(status);
  // репорт в работе у админа:
  state[idx].blocked  = Boolean(blockedBy);
  state[idx].blockedBy= blockedBy;
  // админ окончательно закрыл — удаляем:
  if (closed) {
    state.splice(idx, 1);
  }
  set(state);
},

       deleteReport: (id) => {
      if (global.isAdmin) {
        // АДМИН: удаляем репорт из списка
        const idx = state.findIndex(r => r.id == id);
        if (idx !== -1) {
          state.splice(idx, 1);
          set(state);
        }
      } else {
        // ИГРОК: ставим только флаг закрытия
        window.reportsStore.setStatus(id, 'Закрыто');
      }
    }
  }
  return {
    subscribe,
    init: () => window.reports(),
  };
}

export const reportsData = _reportsDataStore();