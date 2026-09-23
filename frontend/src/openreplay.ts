import { tracker } from "@openreplay/tracker";

const projectKey = import.meta.env.VITE_OPENREPLAY_KEY;

export function startOpenReplay() {
  if (!projectKey) {
    console.warn("OpenReplay: VITE_OPENREPLAY_KEY is not set, session recording is off.");
    return;
  }

  tracker.configure({
    projectKey,
    // OpenReplay normally requires https. Allow http://localhost only while developing.
    __DISABLE_SECURE_MODE: import.meta.env.DEV,
  });

  tracker.start();
}

export { tracker };
