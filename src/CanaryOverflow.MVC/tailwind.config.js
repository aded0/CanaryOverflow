module.exports = {
  content: [
    "./Views/Shared/*.cshtml",
    "./Features/Auth/*.cshtml"
  ],
  theme: {
    extend: {
      width: {
        "50px": "50px",
        "250px": "250px"
      },
      height: {
        "40px": "40px",
        "24px": "24px"
      },
      fill: {
        lemon: '#FEE30D',
        "macaroni-and-cheese": "#FBB17E",
        "black-haze": "#E7ECEA",
        "mine-shaft": "#312E2E"
      },
      fontFamily: {
        roboto: ["Roboto", "sans-serif"]
      }
    },
  }
}
