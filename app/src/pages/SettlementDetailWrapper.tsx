import { useParams, useNavigate } from 'react-router-dom';
import { SettlementDetail } from './SettlementDetail';

export function SettlementDetailWrapper() {
  const { settlementId } = useParams<{ settlementId: string }>();
  const navigate = useNavigate();

  const handleNavigate = (page: string, data?: any) => {
    switch (page) {
      case 'dashboard':
        navigate('/dashboard');
        break;
      case 'addExpense':
        navigate(`/settlement/${settlementId}/add-expense`);
        break;
      case 'expenseDetail':
        navigate(`/expense/${data?.id}`);
        break;
      default:
        navigate('/dashboard');
    }
  };

  return <SettlementDetail settlementId={settlementId || '1'} onNavigate={handleNavigate} />;
}