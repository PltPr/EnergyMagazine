import axios from "axios"
import type { EnergyPricePoint } from "../Models/EnergyPricePoint"

export const getEnergyPrice = async()=>{
    try{
        const response = await axios.get<EnergyPricePoint[]>("http://localhost:5002/api/Energy/AllDayEnergy")
        return response.data
    }catch(err){
        throw err;
    }
}