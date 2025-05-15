import axios from "axios"
import type { EnergyLowestPrice, EnergyPricePoint } from "../Models/EnergyPricePoint"

export const getEnergyPrice = async()=>{
    try{
        const response = await axios.get<EnergyPricePoint[]>("http://localhost:5002/api/Energy/AllDayEnergy")
        return response.data
    }catch(err){
        throw err;
    }
}


export const getLowetPrice = async()=>{
    try{
        const response = await axios.get<EnergyLowestPrice[]>("http://localhost:5002/api/Energy/Extremes")
        return response.data
    }catch(err)
    {
        throw err;
    }
}