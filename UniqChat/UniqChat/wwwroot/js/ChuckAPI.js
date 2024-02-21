// ChuckAPI.js
import { MY_API_KEY } from './ApiKey.js';

//GET JOKE 
async function getJoke() {
    try {
        const response = await fetch("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random", {
            method: "GET",
            headers: {
                "x-rapidapi-host": "matchilling-chuck-norris-jokes-v1.p.rapidapi.com",
                "x-rapidapi-key": MY_API_KEY,
                "accept": "application/json"
            }
        });
        const data = await response.json();
        return data.value;
    } catch (error) {
        console.error(error);
        throw error;
    }
}
export { getJoke };
