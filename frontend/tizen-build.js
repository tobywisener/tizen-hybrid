import { exec, spawn } from "child_process";

var CERTIFICATE_PROFILE = 'chs-ozone';

var cmd = await spawn(
`tizen package -t wgt -s chs-ozone`,
);

await exec(`tizen package -t wgt -s chs-ozone -r ../backend/debug/tv-samsung-6.5/org.tizen.example.TizenDotNet1-1.0.0.tpk -- staging/Example.wgt`);
//if (opts.install) {
    cmd = await exec(`tizen install --name Example.wgt`);
//}

/*

// Build the hybrid package by bundling the backend app

*/

