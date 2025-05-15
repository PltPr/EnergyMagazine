
import type { EnergyLowestPrice, EnergyPricePoint } from '../Models/EnergyPricePoint'

import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
  Scatter
} from 'recharts'

interface Props {
  energyData:EnergyPricePoint[]
  lowestPrice:EnergyLowestPrice[]
}


const Chart = ({ energyData, lowestPrice }: Props) => {
  

  const hourFormatter = new Intl.DateTimeFormat('pl-PL', {
    timeZone: 'Europe/Warsaw',
    hour: '2-digit',
    hour12: false,
  })

  const fullFormatter = new Intl.DateTimeFormat('pl-PL', {
    timeZone: 'Europe/Warsaw',
    weekday: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })

  


  return (
    <div style={{ width: '100%', height: 400 }}>
      <ResponsiveContainer>
        <LineChart data={energyData}>
          <CartesianGrid stroke="#ccc" strokeDasharray="5 5" />
          <XAxis
            dataKey="dateTime"
            tickFormatter={(value) => hourFormatter.format(new Date(value))}
          />
          <YAxis dataKey="price" unit="€" />
          <Tooltip
            labelFormatter={(label) =>
              fullFormatter.format(new Date(label))
            }
            formatter={(value: number) => [`${value.toFixed(2)} EUR`, 'Price']}
          />
          <Line
            type="linear"
            dataKey="price"
            stroke="#8884d8"
          />
          
          
        </LineChart>
      </ResponsiveContainer>
    </div>
  )
}

export default Chart
