const esbuild = require("esbuild");

return esbuild.build({
  entryPoints: [".\\Views\\application.css"],
  bundle: true,
  outfile: ".\\wwwroot\\styles.css"
});
