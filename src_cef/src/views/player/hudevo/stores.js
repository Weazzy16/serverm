import { writable } from 'svelte/store';
import {executeClientToGroup} from "api/rage";

export const currentWeather = writable("thunder");
