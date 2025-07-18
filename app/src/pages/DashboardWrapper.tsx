import { useNavigate } from 'react-router-dom';
import { Dashboard } from './Dashboard';
import { removeAuthToken } from '@/lib/api';

export function DashboardWrapper() {
  const navigate = useNavigate();

  const handleNavigate = (page: string, data?: any) => {
    switch (page) {
      case 'settlement':
        navigate(`/settlement/${data?.id}`);
        break;
      case 'createSettlement':
        navigate('/create-settlement');
        break;
      case 'joinSettlement':
        navigate('/join-settlement');
        break;
      default:
        navigate('/dashboard');
    }
  };

  const handleLogout = () => {
    removeAuthToken();
    navigate('/');
  };

  return <Dashboard onNavigate={handleNavigate} onLogout={handleLogout} />;
}