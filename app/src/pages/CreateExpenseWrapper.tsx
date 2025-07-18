import { useParams, useNavigate } from 'react-router-dom';
import { CreateExpense } from './CreateExpense';

export function CreateExpenseWrapper() {
  const { settlementId } = useParams<{ settlementId: string }>();
  const navigate = useNavigate();

  const handleNavigate = (page: string, data?: any) => {
    switch (page) {
      case 'settlement':
        navigate(`/settlement/${settlementId}`);
        break;
      case 'dashboard':
        navigate('/dashboard');
        break;
      default:
        navigate('/dashboard');
    }
  };

  return <CreateExpense settlementId={settlementId || '1'} onNavigate={handleNavigate} />;
}