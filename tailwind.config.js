/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Pages/**/*.cshtml',
    './Views/**/*.cshtml'
  ],
  theme: {
    extend: {
      backgroundImage: {
        'worldmap-background': "url('/Assets/worldmap.png')",
      }
    },
  },
  plugins: [],
}

