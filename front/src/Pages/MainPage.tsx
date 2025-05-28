import React, { useEffect, useState } from 'react'
import Chart from '../Components/Chart'
import type { EnergyLowestPrice, EnergyPricePoint } from '../Models/EnergyPricePoint'
import { getEnergyPrice, getLowetPrice, getPredictedPrice } from '../Api/EnergyPriceApi'
import LowestPricesTable from '../Components/LowestPricesTable'

type Props = {}

const MainPage = (props: Props) => {
  const [data, setData] = useState<EnergyPricePoint[]>()
  const [lowest, setLowest] = useState<EnergyLowestPrice[]>()
  const[predicted,setPredicted] = useState<EnergyPricePoint[]>()

  useEffect(() => {
    const getData = async () => {
      const value = await getEnergyPrice()
      if (value) setData(value)

      const lowestValue = await getLowetPrice()
      if(lowestValue) setLowest(lowestValue)
    }
    getData()
  }, [])

  useEffect(()=>{
    const getData = async ()=>{
      const value = await getPredictedPrice()
      if(value)setPredicted(value)
    }
    getData()
  },[])
  console.log("pred: ")
  console.log(predicted)
  console.log("data: ")
  console.log(data)

  return (
  <>
    {data && data.length > 0 && (
      <Chart energyData={data} />
    )}

    {lowest && lowest.length > 0 ? (
      <LowestPricesTable data={lowest} />
    ) : (
      <p>Ładowanie danych...</p>
    )}

    <p className="mt-5 font-bold">Predict:</p>

    {predicted && predicted.length > 0 ? (
      <Chart energyData={predicted} />
    ) : (
      <p>Ładowanie danych prognozy...</p>
    )}
  </>
)
}

export default MainPage