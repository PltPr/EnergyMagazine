import React, { useEffect, useState } from 'react'
import Chart from '../Components/Chart'
import type { EnergyLowestPrice, EnergyPricePoint } from '../Models/EnergyPricePoint'
import { getEnergyPrice, getLowetPrice } from '../Api/EnergyPriceApi'

type Props = {}

const MainPage = (props: Props) => {
  const [data, setData] = useState<EnergyPricePoint[]>()
  const [lowest, setLowest] = useState<EnergyLowestPrice[]>()

  useEffect(() => {
    const getData = async () => {
      const value = await getEnergyPrice()
      if (value) setData(value)

      const lowestValue = await getLowetPrice()
      if(lowestValue) setLowest(lowestValue)
    }
    getData()
  }, [])
  console.log(lowest);

  return (
    <>
    <Chart energyData={data ?? []} lowestPrice={lowest ?? []}/>
    </>
  )
}

export default MainPage