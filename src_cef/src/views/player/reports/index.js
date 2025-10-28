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
                closed: false,
                messages: [{ author: author_, text: text_ }]
            });
            set(state);
        },
        setStatus: (id, name) => {
            const index = state.findIndex(r => r.id == id);
            if (state[index]) {
                if (name.length === 0) {
                    state[index].blocked = false;
                    state[index].blockedBy = "";
                } else {
                    state[index].blocked = true;
                    state[index].blockedBy = name;
                }
                set(state);
            }
        },

        deleteReport: (id) => {
            const index = state.findIndex(r => r.id == id);
            if (!state[index]) return;

            state.splice(index, 1);
            set(state);
            },
            closeReport: (id) => {
            const index = state.findIndex(r => r.id == id);
            if (!state[index]) return;
            state[index].closed = true;
            set(state);
        },
        addMessage: (id, author, text_) => {
            const index = state.findIndex(r => r.id == id);
            if (!state[index]) return;
            text_ = format("parseDell", text_);
            if (!state[index].messages) state[index].messages = [];
            state[index].messages.push({ author, text: text_ });
            set(state);
             },
        clear: () => {
            state = [];
            set(state);
        }
    }  
	return {
        subscribe,
        init: () => window.reports (),
	};
}

export const reportsData = _reportsDataStore();