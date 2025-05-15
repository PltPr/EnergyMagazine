import type { EnergyLowestPrice } from '../Models/EnergyPricePoint';

interface Props {
  data: EnergyLowestPrice[];
}

const LowestPricesTable = ({ data }: Props) => {
  const formatter = new Intl.DateTimeFormat('pl-PL', {
    weekday: 'long',
    hour: '2-digit',
    minute: '2-digit',
    timeZone: 'Europe/Warsaw',
  });
  console.log("here",data)
  
   return (
    <div className="overflow-x-auto">
      <table className="min-w-full border border-gray-200 shadow rounded-lg">
        <thead className="bg-gray-100 text-gray-700">
          <tr>
            <th className="px-4 py-2 text-left">Data i godzina</th>
            <th className="px-4 py-2 text-left">Cena (EUR)</th>
          </tr>
        </thead>
        <tbody>
          {data.map((point, idx) => (
            <tr key={idx} className="border-t">
              <td className="px-4 py-2">{formatter.format(new Date(point.dateTime))}</td>
              <td className="px-4 py-2">{point.price.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LowestPricesTable;
