
import type { EnergyLowestPrice, EnergyPricePoint } from '../Models/EnergyPricePoint'

import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ResponsiveContainer,
  ReferenceLine,
  Legend
} from 'recharts'

interface Props {
  energyData:EnergyPricePoint[]
}


const Chart = ({ energyData}: Props) => {
  

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

  
  const referencePrice = 117

  return (
    <div className='w-100% h-100 mr-10 mt-5  bg-gray-300'>
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
            stroke="blue"
          />
           <ReferenceLine
            name="Oszacowana cena czystego prądu PGE G11"
            y={referencePrice}
            stroke="red"
            strokeDasharray="3 3"
            label={{ value: 'Taryfa G11 (PGE)', position: 'right', fill: 'gray' }}
          />
          <Legend 
          payload={[
            { value: 'Ceny energii', type: 'line', id: 'line-price', color: 'blue' },
            { value: 'Oszacowana cena "czystego" prądu PGE G11', type: 'line', id: 'reference-line', color: 'red'},
          ]} 
        />

          
        </LineChart>
      </ResponsiveContainer>
    </div>
  )
}

export default Chart
