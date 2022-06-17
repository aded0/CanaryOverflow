const esbuild = require("esbuild");

return esbuild.build({
  entryPoints: [".\\Views\\application.js"],
  bundle: true,
  outfile: ".\\wwwroot\\bundle.js"
});
