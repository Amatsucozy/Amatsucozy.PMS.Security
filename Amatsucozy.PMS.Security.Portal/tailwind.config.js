/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,css,pcss}",
    "./Areas/**/*.{cshtml,css,pcss}",
    "./wwwroot/css/site.pcss",
  ],
  theme: {
    extend: {},
    fontFamily: {
      sans: ['Inter', 'Roboto', 'sans-serif'],
    }
  },
  plugins: [],
  corePlugins: {
    preflight: false,
  }
}
  