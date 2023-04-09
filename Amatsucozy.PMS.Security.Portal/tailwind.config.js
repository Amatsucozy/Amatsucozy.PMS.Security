/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Pages/**/*.{cshtml,css,pcss}",
        "./Areas/**/*.{cshtml,css,pcss}",
        "./wwwroot/css/site.pcss",
    ],
    theme: {
        extend: {
            colors: {
                primary: '#3f51b5',
                accent: '#ff4081',
                warn: '#f44336'
            }
        },
        fontFamily: {
            sans: ['Poppins', 'Inter', 'Roboto', 'sans-serif'],
        }
    },
    plugins: [],
    corePlugins: {
        preflight: false,
    }
}
  
