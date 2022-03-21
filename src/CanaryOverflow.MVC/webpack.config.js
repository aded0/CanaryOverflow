const {resolve} = require("path");

module.exports = {
  entry: resolve(__dirname, "frontend", "js", "index.ts"),
  mode: "development",
  devtool: "eval-source-map",
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: "ts-loader",
        exclude: /node_modules/
      }
    ]
  },
  resolve: {
    extensions: [".ts"]
  },
  output: {
    filename: "bundle.js",
    path: resolve(__dirname, "wwwroot", "js")
  }
}
