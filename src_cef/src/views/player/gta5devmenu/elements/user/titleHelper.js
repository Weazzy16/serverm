// titleHelper.js

import { vehicleTitles } from "./vehicleTitles.js";

export function getVehicleTitle(modelKey) {
  const entry = vehicleTitles[modelKey];
  if (!entry) return modelKey;

  const hasSafe = !!entry.safeTitle;
  let title = hasSafe ? entry.safeTitle : entry.title;

  if (hasSafe) title = `[RL] ${title}`;

  return title;
}
    