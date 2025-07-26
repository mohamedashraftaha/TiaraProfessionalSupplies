/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#2c479e', // Blue color for primary elements
          dark: '#1565C0',
          light: '#64B5F6',
        },
        secondary: {
          DEFAULT: '#2c479e', // Teal color for secondary elements
          dark: '#00796B',
          light: '#80CBC4',
        },
      },
      fontFamily: {
        sans: ['"Sequel Sans Disp"', 'sans-serif'],
      },
    },
  },
  plugins: [],
}